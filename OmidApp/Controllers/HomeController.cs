using System.Diagnostics;
using System.Security.Claims;
using Infrastrcture.Migrations;
using Kavenegar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmidApp.Models;
namespace OmidApp.Controllers;
[Authorize]
public class HomeController : Controller
{

    private readonly IUser dbuser;
    private readonly IWalet dbWalet;

    private readonly Context db;
    public HomeController(IUser _dbuser, IWalet _dbWalet, Context _db)
    {

        dbuser = _dbuser;
        dbWalet = _dbWalet;
        db = _db;
    }

    public IActionResult ShowCard()
    {
        ViewBag.Cards = db.Cards.ToList();
        return View("Cards");
    }

    public IActionResult aboutme()
    {
        ViewBag.txt = db.Text.Find(1).text;
        return View();
    }

    public async Task<IActionResult> home()
    {
        ViewBag.n1 = db.Phones.Find(1).Phone;
        ViewBag.n2 = db.Phones.Find(2).Phone;
        //get count request
        var id = User.Identity.GetId();

        //check id is null return to login
        if (id == null)
        {
            return RedirectToAction("login", "phone");
        }


        var user = dbuser.ShowUser(Convert.ToInt32(id));


        int result = user.use % user.free;
        if (user.use == 0)
        {
            ViewBag.Mondeh = user.free;
        }
        else if (result == 0 && user.use != 0)
        {
            ViewBag.Mondeh = 0;

        }
        else
        {
            if (user.use > user.free)
            {
                ViewBag.Mondeh = user.free - result + 1;
            }
            else
            {
                ViewBag.Mondeh = user.free - result;
            }


        }
        ViewBag.description = user.description;

        //context request table 
        int sum = 0;
        var requests = db.Requests.Where(x => x.Mony == true).ToList();
        foreach (var item in requests)
        {
            var orders = db.Orders.Where(x => x.IdRequest == item.Id).ToList();
            foreach (var order in orders)
            {
                sum += order.Price;
            }
        }
        ViewBag.Sum = sum;




        return View();
    }











    public IActionResult aboutus()
    {
        //TODO: Implement Realistic Implementation
        return View();
    }

    public IActionResult load()
    {
        // TODO: Your code here
        return View();
    }

    public IActionResult walet()
    {
        var id = User.Identity.GetId();
        var user = dbuser.ShowUser(Convert.ToInt32(id));
        if (user.Cart != "agent")
        {
            ViewBag.ListAgent = dbuser.ShowAllUseragent("");
        }
        return View();
    }


    public IActionResult ShowWalet()
    {
        // TODO: Your code here
        //   updateclaimAsync();
        var id = User.Identity.GetId();
        //   var user=dbuser.ShowUser(Convert.ToInt32(id));
        //   ViewBag.tel=user.Phone;
        // var id = HttpContext.Session.GetString("id");
        var user = dbuser.ShowUser(Convert.ToInt32(id));
        var result = 0;
        foreach (var walet in user.walets)
        {
            result += walet.variz;
            result -= walet.bardasht;
        }
        ViewBag.Walet = result;
        ViewBag.Walets = user.walets.OrderByDescending(x => x.Id);
        return View();
    }

    public IActionResult charge(string txt)
    {
        // TODO: Your code here
        // find phone user equal txt

        ViewBag.User = dbuser.ShowAllUser(txt);

        return View();
    }

    public IActionResult chargeuser(int id)
    {
        // TODO: Your code here

        var user = dbuser.ShowUser(id);
        ViewBag.User = user;
        //id add to session
        HttpContext.Session.SetString("id", id.ToString());

        return View();
    }

    public IActionResult chargeproccess(int txt)
    {
        // TODO: Your code here
        //get my phone
        var idme = User.Identity.GetId();
        var me = dbuser.ShowUser(Convert.ToInt32(idme));

        //get id from session
        var id = HttpContext.Session.GetString("id");
        var user = dbuser.ShowUser(Convert.ToInt32(id));
        //add walet
        var r = dbWalet.AddWalet(me, txt, user);
        if (r)
        {

            return RedirectToAction("ShowWalet", "home");
        }
        else
        {
            TempData["msg"] = "موجودی ناکافی است";
            return RedirectToAction("chargeuser", "home", new { id = id });
        }



    }

    public async Task updateclaimAsync()
    {
        // TODO: Your code here
        var id = User.Identity.GetId();
        var user = dbuser.ShowUser(Convert.ToInt32(id));

        var identity = (ClaimsIdentity)User.Identity;
        var claim = identity.FindFirst("cart");
        var claimwalet = identity.FindFirst("walet");

        if (claim.Value != "0")
        {
            identity.RemoveClaim(claim);
            identity.AddClaim(new Claim("cart", user.Cart));
        }


        if (claimwalet.Value != "0")
        {
            identity.RemoveClaim(claimwalet);
            identity.AddClaim(new Claim("walet", dbWalet.ShowMojodi(user.Phone).ToString()));
        }







        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(principal);

    }


    public IActionResult send()
    {
        // TODO: Your code here
        return View();
    }


    public IActionResult cat(int id)
    {
        //viewbag list category parentid =0 use  db
        ViewBag.ListCategory = db.Categories.Where(a => a.ParentId == 0 && a.MenuId == id).ToList();
        return View();
    }
    public IActionResult Menus()
    {

        ViewBag.ListMenus = db.Users.Include(x => x.UserMenus).ThenInclude(x => x.Menu).FirstOrDefault(x => x.Id == Convert.ToInt32(User.Identity.GetId())).UserMenus.Select(x => x.Menu).ToList();
        return View();
    }

    public IActionResult Car(int ParentId)
    {
        //viewbag list category parentid =0 use  db
        ViewBag.ListCategory = db.Categories.Where(a => a.ParentId == ParentId).ToList();
        return View();
    }


    public IActionResult mainservice(int id)
    {
        if (id != 0)
        {

            System.Console.WriteLine("mainservice id:" + id);
            HttpContext.Session.SetInt32("idcar", id);
        }
        int MenuId = db.Categories.Find(id).MenuId.Value;

        ///session add idcar


        //if services is null
        if (db.Services.Count() == 0)
        {
            db.Services.Add(new Service { Srvicename = "موتور", Parentid = 0, Status = "فعال", MenuId = 1 });
            db.Services.Add(new Service { Srvicename = "گیربکس", Parentid = 0, Status = "فعال", MenuId = 1 });

        }

        db.SaveChanges();
        ViewBag.Services = db.Services.Where(x => x.Parentid == 0 && x.MenuId == MenuId && (x.CatId == id || x.CatId == null)).ToList();



        return View();
    }

    public IActionResult Inviter()
    {
        var id = Convert.ToInt32(User.Identity!.GetId());
        var Menus = db.Users.Include(x => x.UserMenus).ThenInclude(x => x.Menu).ThenInclude(x => x.Admin).FirstOrDefault(x => x.Id == id).UserMenus.Select(x => x.Menu).ToList();
        return View(Menus);
    }

    [HttpPost]
    public IActionResult AddMenu(string Code)
    {
        Menu menu = db.Menus.FirstOrDefault(x => x.Code == Code.Trim());
        if (menu == null)
        {
            TempData["msg"] = "کد معرف صحیح نمیباشد .";
            return RedirectToAction("Inviter");
        }
        var id = Convert.ToInt32(User.Identity!.GetId());

        if (db.UserMenus.Any(x => x.UserId == id && x.MenuId == menu.Id))
        {
            TempData["msg"] = "کد استفاده شده است .";
            return RedirectToAction("Inviter");
        }

        db.UserMenus.Add(new UserMenu()
        {
            MenuId = menu.Id,
            UserId = id
        });
        db.SaveChanges();
        return RedirectToAction("Inviter");
    }

    public IActionResult DelMenu(int id)
    {
        var UserId = Convert.ToInt32(User.Identity!.GetId());
        var menu = db.UserMenus.FirstOrDefault(x => x.MenuId == id && x.UserId == UserId);
        if (menu != null)
        {
            db.UserMenus.Remove(menu);
            db.SaveChanges();
        }
        return RedirectToAction("Inviter");
    }


    public IActionResult price(int serviceparentid)
    {
        // TODO: Your code here
        int? id = HttpContext.Session.GetInt32("idcar");
        System.Console.WriteLine("idcar:" + id);

        if (id == null)
        {
            // Handle the case where the session variable is not set
            return RedirectToAction("Index", "Home"); // or wherever you want to redirect
        }

        var category = db.Categories.Find(id.Value);
        if (category == null)
        {
            // Handle show error
            //log error
            System.Console.WriteLine(id.Value);
            System.Console.WriteLine("Category not found");
            return NotFound();


        }

        ViewBag.carname = category.CatName;

        var services = db.Services.Where(x => x.Parentid == serviceparentid).ToList();
        var prices = db.Prices.Where(p => p.carId == id.Value).ToDictionary(p => p.IdService, p => p.PriceValue);
        List<Service> result = new List<Service>();
        foreach (var service in services)
        {
            if (prices.TryGetValue(service.Id, out int price))
            {
                service.Price = price;
                if (price != 0) result.Add(service);
            }
        }

        ViewBag.Services = result;

        var parentService = db.Services.Find(serviceparentid);
        if (parentService == null)
        {
            // Handle the case where the parent service is not found
            System.Console.WriteLine("Parent service not found");
            return NotFound();
        }

        ViewBag.Srvicename = parentService.Srvicename;

        return View();
    }


    [HttpPost]
    public IActionResult ProcessSelectedServices([FromBody] SelectedServicesModel model)
    {


        //use Transaction
        var Request = new Request();
        using (var transaction = db.Database.BeginTransaction())
        {



            var idcar = HttpContext.Session.GetInt32("idcar");
            if (model == null || model.SelectedServices == null || model.SelectedServices.Count == 0)
            {
                return BadRequest("No services selected.");
            }
            else
            {

                Request.CreateDate = DateTime.Now;
                Request.Status = "پیش فاکتور";
                //get id  NameIdentifier, quser.Id.ToString()),
                var id = User.Identity.GetId();
                Request.UserId = Convert.ToInt32(id);

                //car name get seeionidcar quesry
                var category = db.Categories.Find(idcar.Value);
                Request.CarName = category.CatName;
                Request.MenuId = category.MenuId.Value;

                //find parendname one model data
                var parentService = db.Services.Find(model.SelectedServices[0]).Parentid;
                Request.ParentServiceName = db.Services.Find(parentService).Srvicename;
                Request.Description = "";
                db.Requests.Add(Request);
                db.SaveChanges();
            }

            // Here you can process the selected service IDs
            foreach (var serviceId in model.SelectedServices)
            {
                // Example: Retrieve service details from the database
                var service = db.Services.Find(serviceId);
                if (service != null)
                {
                    var order = new Orders();
                    order.ChildServiceName = service.Srvicename;
                    //price query table price
                    var price = db.Prices.FirstOrDefault(p => p.IdService == service.Id && p.carId == idcar.Value);
                    order.Price = price.PriceValue;
                    order.IdRequest = Request.Id;
                    db.Orders.Add(order);
                    db.SaveChanges();

                }
            }

            transaction.Commit();

        }

        // Areturn redirect to action and send request id
        return Ok(new { message = "Request created successfully", requestId = Request.Id });

    }
    public IActionResult RequestDetail(int id)
    {
        var request = db.Requests.Find(id);
        if (request == null)
        {
            return NotFound();
        }

        var user = db.Users.Find(request.UserId);
        var orders = db.Orders.Where(o => o.IdRequest == id).ToList();

        var viewModel = new RequestDetailViewModel
        {
            Request = request,
            User = user,
            Orders = orders,
            TotalPrice = orders.Sum(o => o.Price)
        };

        return View(viewModel);
    }


    [HttpPost]
    public IActionResult SaveAdditionalDescription(int requestId, string description)
    {
        var request = db.Requests.Find(requestId);
        if (request == null)
        {
            return NotFound(new { success = false, message = "Request not found" });
        }

        //sms 
        // try
        // {
        //       KavenegarApi SmsApi = new KavenegarApi("74774D6F6A6D346D2F5279755241674E49735750424655794E6952636F503735");
        //       SmsApi.VerifyLookup("09149501938", "/admin/home/newrequest?id="+requestId.ToString(), "demo");
        // }
        // catch (System.Exception)
        // {


        // }


        request.Description = description;
        request.Status = "در انتظار تایید";

        db.SaveChanges();

        return Ok(new { success = true, message = "Description saved successfully" });
    }



    public IActionResult request()
    {
        var id = User.Identity.GetId();
        var requests = db.Requests.Where(r => r.UserId == Convert.ToInt32(id)).ToList();

        //create list of requestmodel
        List<RequestModel> requestModels = new List<RequestModel>();
        foreach (var request in requests)
        {
            var requestModel = new RequestModel
            {
                Id = request.Id,
                CarName = request.CarName,
                ParentServiceName = request.ParentServiceName,
                Date = request.CreateDate.ToPersianDateString(),
                Status = request.Status
            };
            requestModels.Add(requestModel);
        }
        return View(requestModels.OrderByDescending(r => r.Id));
    }


}


public class SelectedServicesModel
{
    public List<int> SelectedServices { get; set; }
}



public class RequestDetailViewModel
{
    public Request Request { get; set; }
    public User User { get; set; }
    public List<Orders> Orders { get; set; }
    public int TotalPrice { get; set; }
}


// SaveAdditionalDescription

public class RequestModel
{
    public int Id { get; set; }
    public string CarName { get; set; }

    public string ParentServiceName { get; set; }

    public string Date { get; set; }
    public string Status { get; set; }
}
