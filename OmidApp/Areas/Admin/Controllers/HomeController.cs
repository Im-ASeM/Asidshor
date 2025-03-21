using System.Net.Http.Headers;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmidApp.Models;
using Extensions;
using Infrastrcture.Migrations;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class HomeController : Controller
{

    private readonly IUser dbUser;

    private readonly IWalet dbWalet;
    private readonly Context _context;

    public HomeController(IUser _dbUser, IWalet _dbWalet, Context _context)
    {

        dbUser = _dbUser;
        dbWalet = _dbWalet;
        this._context = _context;
    }

    public IActionResult DeleteService(int Id, int returnId)
    {
        var service = _context.Services.Find(Id);
        if (service != null)
        {
            _context.Services.Remove(service);
            _context.SaveChanges();
        }
        return RedirectToAction("mainservice", new { Id = returnId });
    }

    public IActionResult ShowWalet(int id, string txt)
    {
        if (txt != null)
        {
            ViewBag.walets = dbUser.ShowUser(id).walets.Where(x => x.babat.Contains(txt)).OrderByDescending(x => x.Id).ToList();
        }
        else
        {
            ViewBag.walets = dbUser.ShowUser(id).walets.OrderByDescending(x => x.Id).ToList();
        }
        ViewBag.id = id;
        ViewBag.txt = txt;
        return View();
    }
    public IActionResult NewWaletMinus(int id, int bardasht, string babat)
    {
        _context.waletNews.Add(new WaletNew
        {
            babat = babat,
            bardasht = bardasht,
            date = DateTime.UtcNow,
            UserId = id,
            variz = 0
        });
        _context.SaveChanges();
        return RedirectToAction("inWalet", "home", new { Area = "admin" });
    }
    public IActionResult NewWaletM(int id)
    {
        ViewBag.id = id;
        return View();
    }
    public IActionResult NewWaletPlus(int id, int variz, string babat)
    {
        _context.waletNews.Add(new WaletNew
        {
            babat = babat,
            bardasht = 0,
            date = DateTime.UtcNow,
            UserId = id,
            variz = variz
        });
        _context.SaveChanges();
        return RedirectToAction("inWalet", "home", new { Area = "admin" });
    }
    public IActionResult NewWaletP(int id)
    {
        ViewBag.id = id;
        return View();
    }
    public IActionResult NewWalet(int id)
    {
        ViewBag.id = id;
        return View();
    }
    public IActionResult inWalet(string txt)
    {
        ViewBag.txt = txt;
        ViewBag.User = dbUser.ShowAllUser(txt);
        return View();
    }

    public IActionResult SaveAdminMsg(string adminMsg)
    {
        var Check = _context.Text.Find(1);
        Check.text = adminMsg;

        _context.Text.Update(Check);
        _context.SaveChanges();
        return RedirectToAction("index", "home", new { Area = "Admin" });
    }
    public IActionResult AdminMsg()
    {
        ViewBag.txt = _context.Text.Find(1).text;
        return View();
    }

    public IActionResult EditPhone()
    {
        ViewBag.Num1 = _context.Phones.Find(1).Phone;
        ViewBag.Num2 = _context.Phones.Find(2).Phone;
        return View();
    }

    public IActionResult SavePhone(string Phone1, string Phone2)
    {
        var phone1 = _context.Phones.Find(1);
        var phone2 = _context.Phones.Find(2);

        phone1.Phone = Phone1;
        phone2.Phone = Phone2;

        _context.Phones.Update(phone1);
        _context.Phones.Update(phone2);

        _context.SaveChanges();
        return RedirectToAction("index", "home", new { Area = "Admin" });
    }

    public IActionResult Index(string txt)
    {
        var users = dbUser.ShowAllUser(txt);
        
        // فقط کاربران مربوط به مدیر وارد شده را نمایش می‌دهیم
        if (User.FindFirst("Role").Value != "admin")
        {
            int adminID = Convert.ToInt32(User.Identity.GetId());
            Admin admin = _context.Admins.Include(x => x.AdminMenus).FirstOrDefault(x => x.Id == adminID)!;

            // لیست شناسه‌های منوهای مدیر را دریافت می‌کنیم
            List<int> adminMenus = admin.AdminMenus.Count != 0 ? admin.AdminMenus.Select(x => x.Id).ToList() : new List<int>();

            // کاربرانی که با این منوها ارتباط دارند را فیلتر می‌کنیم
            if (adminMenus.Any())
            {
                // دریافت شناسه‌های کاربران منحصربفرد
                var userIds = _context.UserMenus
                    .Where(um => adminMenus.Contains(um.MenuId))
                    .Select(um => um.UserId)
                    .Distinct()
                    .ToList();
                
                // فیلتر کردن لیست کاربران
                users = users.Where(x => userIds.Contains(x.Id)).ToList();
            }
            else
            {
                // اگر مدیر هیچ منویی ندارد، لیست کاربران را خالی می‌کنیم
                users = new List<VmUser>();
            }
        }
        
        ViewBag.User = users;
        return View();
    }
    public IActionResult set(int Id)
    {
        //get user by id context
        var user = _context.Users.Find(Id);
        ViewBag.User = user;
        return View();

    }
    public IActionResult profile()
    {
        //get id  to identity
        var id = User.Identity.GetId();

        //get user by id context
        var user = _context.Admins.Find(int.Parse(id));
        ViewBag.User = user;
        return View();

    }
    public IActionResult setpassword(int id, string password)
    {
        //get user by id context
        var user = _context.Admins.Find(id);
        // user.pa=free;
        user.Password = password;

        _context.Admins.Update(user);
        _context.SaveChanges();
        return RedirectToAction("index");

    }

    public IActionResult setnew(int id, int free, string description)
    {
        //get user by id context
        var user = _context.Users.Find(id);
        user.free = free;

        user.description = description;

        _context.Users.Update(user);
        _context.SaveChanges();
        return RedirectToAction("index");

    }
    public IActionResult login(bool faild = false)
    {
        ViewBag.Error = faild ? "کلمه عبور یا نام کاربری اشتباه است" : null;
        return View();
    }
    public async Task<IActionResult> logAsync(string Password, string email)
    {
        //check if admins is null add default admin and password
        if (_context.Admins.Count() == 0)
        {
            _context.Admins.Add(new Admin { UserName = "admin", Password = "admin", Role = "admin", Status = "فعال", PhoneNumber = "09130495923", NameFamily = "آقای حمزه ای نژاد" });
            _context.SaveChanges();


        }
        if (_context.Phones.Count() == 0)
        {
            _context.Phones.Add(new Phones
            {
                Phone = "09130495923"
            });
            _context.SaveChanges();
            _context.Phones.Add(new Phones
            {
                Phone = "09390916408"
            });
            _context.SaveChanges();
        }

        if (_context.Text.Count() == 0)
        {
            _context.Text.Add(new Text
            {
                text = "اپلیکیشن اسیدشویی آنلاین\nسرویس آنلاین اسیدشویی قطعات برای تعمیرکاران\n\nاپلیکیشن اسیدشویی آنلاین با هدف تسهیل فرآیند اسیدشویی قطعات برای تعمیرکاران در سال 1402 راه‌اندازی شد. ایده اصلی ما این بود که چگونه می‌توانیم زمان و هزینه‌های تعمیرکاران را کاهش دهیم و کیفیت خدمات اسیدشویی را افزایش دهیم."
            });
            _context.SaveChanges();
        }


        var user = _context.Admins.Where(x => x.UserName == email && x.Password == Password && x.Status == "فعال").FirstOrDefault();
        if (user != null)
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[]
             {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    //add new claim for add value quser.cart if null set 0
                    new Claim("Role", user.Role)

                }, CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Response.Cookies.Append("InviteCode", user.InviteCode == null ? "- - -" : user.InviteCode, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            HttpContext.Response.Cookies.Append("NameFamily", user.NameFamily, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddYears(100), // Set the ExpiresUtc to a far-future date
                IsPersistent = true
            };
            await HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("index");

        }
        else
        {

            TempData["error"] = "Error";
            return RedirectToAction("login", new { faild = true });
        }
    }
    public async Task<IActionResult> ParticipantsscoreAsync(string txt)
    {

        try
        {
            DateTime Today = txt.ToGeorgianDateTime();

            return View();
        }
        catch (System.Exception ex)
        {

            return View();
        }



    }
    public IActionResult info(string txt)
    {
        ViewBag.User = dbUser.ShowAllUser(txt);
        return View();
    }

    public IActionResult paystatus(string txt)
    {
        //TODO: Implement Realistic Implementation
        try
        {
            DateTime Today = txt.ToGeorgianDateTime();
            ViewBag.walet = dbWalet.ShowResultAll().Where(x => x.date.Date == Today).OrderByDescending(x => x.Id).ToList();
            ViewBag.Mojodi = dbWalet.ShowResultAll().Where(x => x.date.Date == Today).Sum(x => x.variz);

        }
        catch (System.Exception ex)
        {

            ViewBag.walet = dbWalet.ShowResultAll().OrderByDescending(x => x.Id).ToList();
            ViewBag.Mojodi = dbWalet.ShowResultAll().Sum(x => x.variz);
        }



        return View();
    }
    public IActionResult setting()
    {
        //TODO: Implement Realistic Implementation

        return View();
    }
    public IActionResult bag()
    {
        //TODO: Implement Realistic Implementation
        return View();
    }

    public IActionResult logout()
    {
        HttpContext.SignOutAsync();
        return RedirectToAction("login");
    }

    //agent 
    public IActionResult agentinfo(int id)
    {
        var q = dbUser.ChangeAgent(id);
        return RedirectToAction("index");
    }

    public IActionResult walet(string phone)
    {
        ViewBag.walet = dbWalet.ShowAllWalet(phone);
        //nameresives take 1 into viewbag.walet
        ViewBag.name = dbUser.ShowAllUser(phone).FirstOrDefault().FirstAndLastName + "  با شماره تلفن : " + phone;
        ViewBag.tel = phone;
        ViewBag.mojodi = dbWalet.ShowMojodi(phone);
        //save phone to session
        if (phone != null)
            HttpContext.Session.SetString("phone", phone);
        return View();
    }

    public IActionResult charge(int txt)
    {
        dbWalet.ChargeAccunt(txt, HttpContext.Session.GetString("phone"));
        return RedirectToAction("walet", new { phone = HttpContext.Session.GetString("phone") });
    }


    public IActionResult del(int id)
    {
        // TODO: Your code here
        dbWalet.delete(id);
        return RedirectToAction("walet", new { phone = HttpContext.Session.GetString("phone") });
    }

    public IActionResult deatils(int id)
    {
        var requests = _context.Requests.Where(r => r.UserId == Convert.ToInt32(id)).ToList();
        if (User.FindFirst("Role").Value != "admin")
        {
            int adminID = Convert.ToInt32(User.Identity.GetId());
            // Admin admin = _context.Admins.Include(x => x.City).ThenInclude(x => x.Menu).FirstOrDefault(x => x.Id == adminID);
            List<int> adminMenus = _context.Admins.Include(x => x.AdminMenus).FirstOrDefault(x => x.Id == adminID).AdminMenus.Select(x => x.Id).ToList();
            requests = requests.Where(x => adminMenus.Contains(x.MenuId)).ToList();
        }

        //create list of requestmodel
        List<RequestModel2> requestModels = new List<RequestModel2>();
        foreach (var request in requests)
        {
            var requestModel = new RequestModel2
            {
                Id = request.Id,
                CarName = request.CarName,
                ParentServiceName = request.ParentServiceName,
                Date = request.CreateDate.ToPersianDateString(),
                Status = request.Status
            };
            requestModels.Add(requestModel);
        }
        return View(requestModels);
    }


    public IActionResult agent(string txt)
    {
        // TODO: Your code here
        ViewBag.User = dbUser.ShowAllUseragent(txt);

        return View();
    }

    public IActionResult waletall()
    {
        // TODO: Your code here

        return View();
    }

    public IActionResult AdminSelect(int Id)
    {
        ViewBag.Admins = _context.Admins.Where(x => x.Status == "فعال" && x.Id != 1).ToList();
        ViewBag.MenuId = Id;
        return View();
    }

    public IActionResult SelectAdmin(int adminID, int MenuId)
    {
        Menu menu = _context.Menus.Find(MenuId)!;
        menu.AdminId = adminID;
        menu.Code = _context.Admins.Find(adminID)!.InviteCode;
        _context.Menus.Update(menu);
        _context.SaveChanges();
        return RedirectToAction("addMenu");
    }


    public IActionResult exit()
    {
        // TODO: Your code here
        HttpContext.SignOutAsync();
        return RedirectToAction("login");

    }





    //vige
    public IActionResult vige(int id)
    {
        var user = _context.Users.Find(id);
        if (user.Url == "vige")
        {
            user.Url = "";
        }
        else
        {
            //first delete all vige
            var q = _context.Users.Where(x => x.Url == "vige").ToList();
            if (q.Count() < 4)
            {
                user.Url = "vige";

            }
            else
            {
                TempData["msg"] = "بیش از 4 نفر امکان ویژه بودن را ندارند ";
            }



        }


        _context.Users.Update(user);
        _context.SaveChanges();


        return RedirectToAction("index");

    }



    // category
    public IActionResult category(int Id)
    {

        //check if Categories is empty add default category parent "خودروهای سبک" و"خودروهای سنگین" "سایر"
        if (_context.Menus.Count() == 0)
        {
            _context.Menus.Add(new Menu { Name = "اسیدشویی فوری" });
            _context.SaveChanges();
            _context.Categories.Add(new Category { CatName = "خودروهای سبک", ParentId = 0, Status = "فعال", MenuId = 1 });
            _context.Categories.Add(new Category { CatName = "خودروهای سنگین", ParentId = 0, Status = "فعال", MenuId = 1 });
            _context.SaveChanges();
        }
        ViewBag.Category = _context.Categories.Where(x => x.ParentId == 0 && x.MenuId == Id).ToList();
        ViewBag.Menu = _context.Menus.Find(Id);
        return View();
    }

    public IActionResult addMenu()
    {
        if (_context.Menus.Count() == 0)
        {
            _context.Menus.Add(new Menu { Name = "اسیدشویی فوری", Code = "1" });
            _context.SaveChanges();
            _context.Categories.Add(new Category { CatName = "خودروهای سبک", ParentId = 0, Status = "فعال", MenuId = 1 });
            _context.Categories.Add(new Category { CatName = "خودروهای سنگین", ParentId = 0, Status = "فعال", MenuId = 1 });
            _context.SaveChanges();
        }
        if (User.FindFirst("Role").Value == "admin")
        {
            ViewBag.Menu = _context.Menus.OrderByDescending(x => x.Id).Include(x => x.Admin).ToList();
        }
        else
        {
            // int Userid = HttpContext.Session.GetInt32("id").Value;
            int Userid = Convert.ToInt32(User.Identity.GetId());
            var AdminMenus = _context.Admins.Include(x => x.AdminMenus).FirstOrDefault(x => x.Id == Userid).AdminMenus;

            ViewBag.Menu = AdminMenus;
        }
        return View();
    }
    public IActionResult editMenu(int? id)
    {
        var citys = _context.Cities.ToList();
        ViewBag.city = citys;
        var menu = id.HasValue ? _context.Menus.Include(x => x.City).ThenInclude(x => x.City).FirstOrDefault(x => x.Id == id.Value) : new Menu();
        var viewModel = new MenuCityViewModel
        {
            MenuId = menu.Id,
            MenuName = menu.Name,
            Cities = citys.Select(c => new CitySelection
            {
                CityId = c.Id,
                CityName = c.CityName,
                IsSelected = menu.City != null ? menu.City.Any(mc => mc.CityId == c.Id) : false
            }).ToList(),
            Code = menu.Code
        };

        return View(viewModel);
    }
    [HttpPost]
    public IActionResult editMenu(MenuCityViewModel model)
    {
        Menu menu;
        Random random = new Random();
        if (model.MenuId == 0)
        {
            menu = new Menu
            {
                Name = model.MenuName,
                City = new List<CityMenu>()
            };
            _context.Menus.Add(menu);
        }
        else
        {
            menu = _context.Menus.Include(x => x.City).FirstOrDefault(x => x.Id == model.MenuId);
            menu.Name = model.MenuName;
            menu.City.Clear();
        }

        foreach (var city in model.Cities)
        {
            if (city.IsSelected)
            {
                menu.City.Add(new CityMenu { CityId = city.CityId });
            }
        }

        _context.SaveChanges();
        return RedirectToAction("addMenu");
    }
    public IActionResult DeleteMenu(int id)
    {
        var menu = _context.Menus.Find(id);
        if (menu != null)
        {
            var categories = _context.Categories.Where(x => x.MenuId == id).ToList();
            var services = _context.Services.Where(x => x.MenuId == id).ToList();
            _context.Services.RemoveRange(services);
            _context.Categories.RemoveRange(categories);
            _context.Menus.Remove(menu);
            _context.SaveChanges();
        }
        return RedirectToAction("addMenu");
    }

    [HttpPost]
    public IActionResult AddCategory(Category category)
    {
        category.ParentId = 0;
        category.Status = "فعال";
        _context.Categories.Add(category);
        _context.SaveChanges();
        return RedirectToAction("category", new { Id = category.MenuId });
    }

    public IActionResult DeleteCategory(int Id)
    {
        var category = _context.Categories.Find(Id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("category", new { Id = category.MenuId });
        }
        return RedirectToAction("addMenu");
    }

    public IActionResult AddService(string Name, int MenuId, int ParentId, int catId)
    {
        _context.Services.Add(new Service { Srvicename = Name, Parentid = 0, Status = "فعال", MenuId = MenuId, CatId = catId });
        _context.SaveChanges();
        return RedirectToAction("mainservice", new { id = ParentId });
    }


    //add category
    public IActionResult addcar(string CatName)
    {
        string msg = "";

        //check exist category
        var q = _context.Categories.Where(x => x.CatName == CatName).FirstOrDefault();
        if (q == null)
        {
            //get session id
            int ParentId = HttpContext.Session.GetInt32("id").Value;


            _context.Categories.Add(new Category { CatName = CatName, ParentId = ParentId, Status = "فعال", MenuId = _context.Categories.Find(ParentId).MenuId });
            _context.SaveChanges();
            msg = "دسته بندی با موفقیت ثبت شد";
        }
        else
        {
            msg = "این دسته بندی قبلا ثبت شده است";
        }

        // writed by mhd
        //add a fild to price table with  carId 
        //find carId by CatName
        var find = _context.Categories.Where(x => x.CatName == CatName).FirstOrDefault();
        _context.Prices.Add(new Price { carId = find.Id });
        _context.SaveChanges();


        //get id by session
        return RedirectToAction("car", new { id = HttpContext.Session.GetInt32("id"), meesage = msg });


    }


    public IActionResult Car(int id, string? meesage)
    {
        //add session id
        HttpContext.Session.SetInt32("id", id);
        //viewbad listcars
        ViewBag.listcars = _context.Categories.Where(x => x.ParentId == id).ToList();
        //viewbad catname
        ViewBag.catname = _context.Categories.Find(id).CatName;
        //viewbad id
        ViewBag.id = id;
        //viewbad meesage
        ViewBag.meesage = meesage;
        return View();
    }
    public IActionResult EditCar(int id)
    {
        //viewbad catname
        ViewBag.catname = _context.Categories.Find(id).CatName;
        //viewbad id
        ViewBag.id = id;
        return View();
    }

    public IActionResult saveCar(int id, string CatName)
    {
        Category check = _context.Categories.Find(id);
        check.CatName = CatName;
        _context.Categories.Update(check);
        _context.SaveChanges();
        return RedirectToAction("car", "home", new { Area = "admin", id = HttpContext.Session.GetInt32("id") });
    }

    public IActionResult Service()
    {

        return View();



    }


    // update price
    [HttpPost]
    public IActionResult ServiceUpdate(Price price)
    {
        var find = _context.Prices.Where(x => x.carId == price.carId).FirstOrDefault();
        if (find != null)
        {

        }
        return RedirectToAction("Service");
    }



    //catdelete
    public IActionResult Catdelete(int id)

    {


        //delete all  with by id
        var q = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
        _context.Categories.Remove(q);
        _context.SaveChanges();

        //delete all prices with carId
        var q1 = _context.Prices.Where(x => x.carId == id).FirstOrDefault();
        _context.Prices.Remove(q1);
        _context.SaveChanges();

        string msg = "دسته بندی با موفقیت حذف شد";



        return RedirectToAction("Car", new { id = HttpContext.Session.GetInt32("id"), meesage = msg });

    }


    //mainservice
    public IActionResult mainservice(int id)
    {

        ///session add idcar
        HttpContext.Session.SetInt32("idcar", id);
        var Parent = _context.Categories.Find(id)!;

        //if services is null
        if (_context.Services.Count() == 0)
        {
            _context.Services.Add(new Service { Srvicename = "موتور", Parentid = 0, Status = "فعال", MenuId = 1 });
            _context.Services.Add(new Service { Srvicename = "گیربکس", Parentid = 0, Status = "فعال", MenuId = 1 });
            _context.SaveChanges();
        }
        ViewBag.Parent = Parent;
        ViewBag.Services = _context.Services.Where(x => x.Parentid == 0 && x.MenuId == Parent.MenuId && (x.CatId == id || x.CatId == null)).ToList();
        ViewBag.Id = id;

        return View();
    }

    //price
    public IActionResult Price(int serviceparentid)
    {
        int? id = HttpContext.Session.GetInt32("idcar");
        int MenuId = Convert.ToInt32(_context.Categories.Find(id.Value).MenuId);

        if (id == null)
        {
            // Handle the case where the session variable is not set
            return RedirectToAction("Index", "Home"); // or wherever you want to redirect
        }

        var category = _context.Categories.Find(id.Value);
        if (category == null)
        {
            // Handle the case where the category is not found
            return NotFound();
        }

        ViewBag.carname = category.CatName;
        ViewBag.MenuId = MenuId;
        ViewBag.id = serviceparentid;

        var services = _context.Services.Where(x => x.Parentid == serviceparentid).ToList();
        var prices = _context.Prices.Where(p => p.carId == id.Value).ToDictionary(p => p.IdService, p => new { p.PriceValue, p.Quantity });

        foreach (var service in services)
        {
            if (prices.TryGetValue(service.Id, out var priceInfo))
            {
                service.Price = priceInfo.PriceValue;
                service.Quantity = priceInfo.Quantity;
            }
            else
            {
                service.Price = 0;
                service.Quantity = 0;
            }
        }

        ViewBag.Services = services;

        var parentService = _context.Services.Find(serviceparentid);
        if (parentService == null)
        {
            // Handle the case where the parent service is not found
            return NotFound();
        }

        ViewBag.Srvicename = parentService.Srvicename;
        ViewBag.catId = id.Value;

        return View();
    }

    [HttpPost]
    public IActionResult SavePrices(List<int> ServiceIds, List<int> Prices, List<int> Quantities)
    {
        int carId = HttpContext.Session.GetInt32("idcar").Value;

        for (int i = 0; i < ServiceIds.Count; i++)
        {
            var price = new Price
            {
                carId = carId,
                IdService = ServiceIds[i],
                PriceValue = Prices[i],
                Quantity = Quantities != null && Quantities.Count > i ? Quantities[i] : 0
            };

            // Check if a price already exists for this car and service
            var existingPrice = _context.Prices.FirstOrDefault(p => p.carId == carId && p.IdService == ServiceIds[i]);
            if (existingPrice != null)
            {
                existingPrice.PriceValue = Prices[i];
                existingPrice.Quantity = Quantities != null && Quantities.Count > i ? Quantities[i] : existingPrice.Quantity;
            }
            else
            {
                _context.Prices.Add(price);
            }
        }

        _context.SaveChanges();

        //get service parent id
        int serviceparentid = _context.Services.Find(ServiceIds[0]).Parentid;

        return RedirectToAction("Price", new { serviceparentid = serviceparentid });
    }

    // get all request and sent with user detalis to front by model

    public IActionResult Request(int page = 1, int pageSize = 10, int? managerId = null)
    {
        var currentUserId = Convert.ToInt32(User.Identity.GetId());
        var currentUserRole = User.FindFirst("Role").Value;
        var isAdmin = currentUserRole == "admin";

        // Create a dictionary to store manager's requests
        var managerRequests = new Dictionary<Admin, List<RequestModel>>();

        // Get requests without manager code (system admin's requests) FIRST
        var unassignedRequests = _context.Requests
            .Where(x => !_context.Menus.Any(m => m.Id == x.MenuId && m.AdminId != null))
            .OrderByDescending(x => x.Id)
            .ToList()
            .Select(request =>
            {
                var user = _context.Users.Find(request.UserId);
                return new RequestModel
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    UserName = user.FirstAndLastName,
                    Adress = user.Adress,
                    UserPhone = user.Phone,
                    tamirgah = user.Url,
                    Date = request.CreateDate.ToPersianDateString(),
                    Status = request.Status,
                    Mony = request.Mony,
                    ManagerName = "مدیر سیستم",
                    ManagerCode = "SYSTEM"
                };
            }).ToList();

        if (unassignedRequests.Any())
        {
            managerRequests.Add(new Admin { NameFamily = "مدیر سیستم", InviteCode = "SYSTEM" }, unassignedRequests);
        }

        // Get all managers
        var managers = _context.Admins
            .Where(x => x.Role == "agent" && x.Status == "فعال")
            .Include(x => x.AdminMenus)
            .ToList();

        foreach (var manager in managers)
        {
            var managerMenuIds = manager.AdminMenus.Select(m => m.Id).ToList();
            var requests = _context.Requests
                .Where(x => managerMenuIds.Contains(x.MenuId))
                .OrderByDescending(x => x.Id)
                .ToList();

            var requestModels = requests.Select(request =>
            {
                var user = _context.Users.Find(request.UserId);
                return new RequestModel
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    UserName = user.FirstAndLastName,
                    Adress = user.Adress,
                    UserPhone = user.Phone,
                    tamirgah = user.Url,
                    Date = request.CreateDate.ToPersianDateString(),
                    Status = request.Status,
                    Mony = request.Mony,
                    ManagerName = manager.NameFamily,
                    ManagerCode = manager.InviteCode
                };
            }).ToList();

            if (requestModels.Any())
            {
                managerRequests.Add(manager, requestModels);
            }
        }

        // Filter by manager if specified
        if (managerId.HasValue)
        {
            managerRequests = managerRequests.Where(x => x.Key.Id == managerId)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        // For non-admin users, only show their own requests
        if (!isAdmin)
        {
            managerRequests = managerRequests.Where(x => x.Key.Id == currentUserId)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        ViewBag.Managers = managers;
        ViewBag.SelectedManagerId = managerId;
        ViewBag.ManagerRequests = managerRequests;

        return View();
    }





    //show request details
    public IActionResult RequestDetails(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(request.UserId);
        var orders = _context.Orders.Where(o => o.IdRequest == id).ToList();
        var viewModel = new RequestDetailViewModel
        {
            Request = request,
            User = user,
            Orders = orders,
            TotalPrice = orders.Sum(o => o.Price * o.Quantity)
        };
        ViewBag.Latitude = user.Latitude;
        ViewBag.Longitude = user.Longitude;

        return View(viewModel);
    }

    //accept request
    public IActionResult AcceptRequest(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(request.UserId);

        user.use += 1;
        _context.SaveChanges();

        var userme = _context.Users.Find(request.UserId);
        //if user use > user free 

        if (userme.use != 0 && userme.free != 0 && (userme.use % (userme.free + 1)) == 0)
        {
            request.Mony = true;
        }

        request.Status = "تایید شده";
        _context.SaveChanges();

        //get user by id and mineze free


        return RedirectToAction("Request");
    }

    public IActionResult AcceptRequest2(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        request.Status = "اتمام و ارسال شده ";
        _context.SaveChanges();
        //get user by id and mineze free
        var user = _context.Users.Find(request.UserId);



        return RedirectToAction("Request");
    }

    //reject request
    public IActionResult RejectRequest(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        request.Status = "رد شده";
        _context.SaveChanges();

        return RedirectToAction("Request");
    }

    //delete request and return to last Iaction

    public IActionResult DeleteRequest(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        _context.Requests.Remove(request);
        _context.SaveChanges();

        var orders = _context.Orders.Where(o => o.IdRequest == id).ToList();
        _context.Orders.RemoveRange(orders);
        _context.SaveChanges();

        return RedirectToAction("Request");

    }

    public IActionResult DeleteRequestt(int id)
    {
        var request = _context.Requests.Find(id);
        var user = _context.Users.Find(request.UserId);

        user.use -= 1;

        if (request == null)
        {
            return NotFound();
        }

        _context.Requests.Remove(request);
        _context.SaveChanges();

        var orders = _context.Orders.Where(o => o.IdRequest == id).ToList();
        _context.Orders.RemoveRange(orders);
        _context.SaveChanges();



        return RedirectToAction("deatils", new { id = request.UserId });

    }


    // Path: OmidApp/Models/RequestModel.cs
    public class RequestModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string tamirgah { get; set; }
        public string Adress { get; set; }
        public string UserPhone { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }

        //Mony
        public bool Mony { get; set; }

        public string ManagerName { get; set; }
        public string ManagerCode { get; set; }
    }

    public class RequestListViewModel
    {
        public List<RequestModel> Requests { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRequests { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalRequests / PageSize);
    }




    public class RequestDetailViewModel
    {
        public Request Request { get; set; }
        public User User { get; set; }
        public List<Orders> Orders { get; set; }
        public int TotalPrice { get; set; }
    }

    public class RequestModel2
    {
        public int Id { get; set; }
        public string CarName { get; set; }

        public string ParentServiceName { get; set; }

        public string Date { get; set; }
        public string Status { get; set; }
    }

    public IActionResult NewRequest(int id)
    {
        var request = _context.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        var user = _context.Users.Find(request.UserId);
        var orders = _context.Orders.Where(o => o.IdRequest == id).ToList();
        var viewModel = new RequestDetailViewModel
        {
            Request = request,
            User = user,
            Orders = orders,
            TotalPrice = orders.Sum(o => o.Price)
        };
        ViewBag.Latitude = user.Latitude;
        ViewBag.Longitude = user.Longitude;

        return View(viewModel);
    }

    public IActionResult Listadmin()
    {
        var admins = _context.Admins
            .OrderByDescending(x => x.Id)
            .Include(x => x.City)
            .Include(x => x.AdminMenus)
            .ToList();
        
        // Create a dictionary to store admin and their user counts
        var adminUserCounts = new Dictionary<int, int>();
        
        foreach (var admin in admins)
        {
            // Count users for system admin (ID 1) differently - include all users
            if (admin.Id == 1 && admin.Role == "admin")
            {
                adminUserCounts.Add(admin.Id, _context.Users.Count());
                continue;
            }
            
            // Get the menu IDs associated with this admin
            var adminMenuIds = admin.AdminMenus?.Select(m => m.Id).ToList() ?? new List<int>();
            
            // Count users who have any of these menus
            int userCount = 0;
            if (adminMenuIds.Any())
            {
                // Use the same logic as in AdminUsers method
                var userIds = _context.UserMenus
                    .Where(um => adminMenuIds.Contains(um.MenuId))
                    .Select(um => um.UserId)
                    .Distinct()
                    .ToList();
                
                userCount = userIds.Count;
            }
            
            adminUserCounts.Add(admin.Id, userCount);
        }
        
        ViewBag.User = admins;
        ViewBag.AdminUserCounts = adminUserCounts;
        
        return View();
    }

    public IActionResult Active(int Id)
    {

        //get user by id context
        var user = _context.Admins.Find(Id);
        if (user.Status == "فعال")
        {
            user.Status = "غیر فعال";
        }
        else
        {
            user.Status = "فعال";
        }
        _context.Admins.Update(user);
        _context.SaveChanges();

        // TODO: Your code here
        return RedirectToAction("Listadmin");
    }

    //delete admin
    public IActionResult Deleteadmin(int Id)
    {
        var user = _context.Admins.Include(x => x.AdminMenus).ThenInclude(x => x.UserMenus).Include(x => x.City).FirstOrDefault(x => x.Id == Id);
        //if name sdmin not delet
        if (user!.UserName == "admin")
        {

            return RedirectToAction("Listadmin");
        }
        //get user by id context
        foreach (Menu item in user.AdminMenus)
        {
            item.Code = "";
            item.UserMenus.Clear();
            _context.Menus.Update(item);
        }
        _context.Admins.Remove(user);
        _context.SaveChanges();

        return RedirectToAction("Listadmin");
    }

    public IActionResult addadmin()
    {
        // TODO: Your code here
        return View();
    }

    [HttpPost]
    public IActionResult addadmin(Admin admin)
    {
        //if exist username 
        var q = _context.Admins.Where(x => x.UserName == admin.UserName).FirstOrDefault();
        if (q == null)
        {
            Random random = new Random();
            do
            {
                admin.InviteCode = random.Next(100000, 999999).ToString();
            } while (_context.Admins.Any(x => x.InviteCode == admin.InviteCode));

            //status
            admin.Status = "فعال";
            //role
            admin.Role = "agent";
            _context.Admins.Add(admin);
            _context.SaveChanges();
        }
        else
        {
            TempData["msg"] = "نام کاربری قبلا ثبت شده است";
            return View();
        }
        return RedirectToAction("Listadmin");
    }


    public IActionResult editadmin(int id)
    {
        //get user by id context
        var user = _context.Admins.Find(id);

        // TODO: Your code here
        return View(user);
    }

    [HttpPost]
    public IActionResult editadmin(Admin admin)
    {
        //check if username already exists for another admin
        if (_context.Admins.Any(x => x.UserName == admin.UserName && x.Id != admin.Id))
        {
            TempData["msg"] = "نام کاربری قبلا توسط مدیر دیگری استفاده شده است";
            return View(admin);
        }

        //update user just change iteams
        var user = _context.Admins.Find(admin.Id);

        user.NameFamily = admin.NameFamily;
        user.PhoneNumber = admin.PhoneNumber;
        user.Password = admin.Password;
        user.UserName = admin.UserName;

        //update
        _context.Admins.Update(user);
        _context.SaveChanges();

        return RedirectToAction("Listadmin");
    }


    public IActionResult CardOfBanks()
    {
        ViewBag.listcards = _context.Cards.OrderByDescending(x => x.Id).ToList();
        // TODO: Your code here
        return View();
    }

    public IActionResult carddelete(int id)
    {
        var card = _context.Cards.Find(id);
        if (card != null)
        {
            _context.Cards.Remove(card);
            _context.SaveChanges();

        }

        return RedirectToAction("CardOfBanks");
    }



    [HttpPost]
    public IActionResult CardOfBanks(Card card)
    {
        if (ModelState.IsValid)
        {
            _context.Cards.Add(card);
            _context.SaveChanges();
            return RedirectToAction("CardOfBanks");
        }
        return View();
    }

    // [HttpPost]
    // public IActionResult AssignCityRegion(string CitytName)
    // {
    //     //get userid from session
    //     var UserId = HttpContext.Session.GetInt32("id");

    //     if (!_context.Cities.Any(x => x.CityName == CitytName && x.UserId == UserId))
    //     {
    //         _context.Cities.Add(new City { CityName = CitytName, UserId = UserId ?? 0 });
    //         _context.SaveChanges();


    //     }
    //     return RedirectToAction("AssignCityRegion", new { id = UserId });
    // }
    [HttpGet]
    public IActionResult AssignCityRegion(int id)
    {
        //id set session
        HttpContext.Session.SetInt32("id", id);
        //get list city by userid
        ViewBag.listcities = _context.Cities.ToList();
        ViewBag.City = _context.Admins.Include(x => x.City).Where(x => x.Id == id).FirstOrDefault()?.City;
        return View();
    }

    // deleteCity
    public IActionResult DeleteCity(int Id)
    {
        var city = _context.Cities.Include(x => x.Admins).FirstOrDefault(x => x.Id == Id);
        if (city != null)
        {
            _context.Cities.Remove(city);
            _context.SaveChanges();

        }
        return RedirectToAction("ListCity");
    }

    public IActionResult ListCity()
    {
        ViewBag.City = _context.Cities.Include(x => x.Admins).Include(x => x.Users).ToList();
        return View();
    }
    public IActionResult AddCity(string CityName)
    {
        if (!_context.Cities.Any(x => x.CityName == CityName))
        {
            _context.Cities.Add(new City { CityName = CityName });
            _context.SaveChanges();
        }
        return RedirectToAction("ListCity");
    }

    public IActionResult CitySelect(int id)
    {
        int Userid = HttpContext.Session.GetInt32("id").Value;

        var UserAdmin = _context.Admins.Find(Userid);
        // id == 0 ? User.CityId = null : User.CityId = id;
        UserAdmin.CityId = id == 0 ? null : id;
        _context.Admins.Update(UserAdmin);
        _context.SaveChanges();

        return RedirectToAction("Listadmin");
    }

    public IActionResult AdminUsers(int id)
    {
        var admin = _context.Admins
            .Include(x => x.AdminMenus)
            .FirstOrDefault(x => x.Id == id);
            
        if (admin == null)
        {
            return NotFound();
        }
        
        // Get the menu IDs associated with this admin
        var adminMenuIds = admin.AdminMenus?.Select(m => m.Id).ToList() ?? new List<int>();
        
        // Get users who have any of these menus
        var users = new List<VmUser>();
        if (adminMenuIds.Any())
        {
            // Get distinct user IDs first
            var userIds = _context.UserMenus
                .Where(um => adminMenuIds.Contains(um.MenuId))
                .Select(um => um.UserId)
                .Distinct()
                .ToList();
                
            // Then get the full user details
            var userEntities = _context.Users
                .Include(u => u.UserMenus)
                .Include(u => u.City)
                .Include(u => u.walets)
                .Where(u => userIds.Contains(u.Id))
                .ToList();
                
            foreach (var user in userEntities)
            {
                users.Add(new VmUser
                {
                    Id = user.Id,
                    FirstAndLastName = user.FirstAndLastName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Url = user.Url,
                    Code = user.Code,
                    Cart = user.Cart,
                    Adress = user.Adress,
                    CityName = user.City?.CityName,
                    
                });
            }
        }
        
        ViewBag.Admin = admin;
        ViewBag.Users = users;
        
        return View();
    }
}

