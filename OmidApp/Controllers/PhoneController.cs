using System.Security.Claims;
using Kavenegar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;





public class PhoneController : Controller
{
   
    private readonly IUser dbuser;
    private readonly IWalet dbWalet;
    private readonly Context _context;


    public PhoneController( IUser _dbuser, IWalet _dbWalet, Context context)
    {
       
        this.dbuser = _dbuser;
        this.dbWalet = _dbWalet;
        this._context = context;
    }


    public IActionResult Login()
    {
      


        if (User.Identity.IsAuthenticated)
            return RedirectToAction("home", "Home");


        return View();

    }

    public IActionResult Check(string phone, string name, string password,string dev,string url,string Adress,string latitude,string longitude)
{
    //check if user exist into _context
    var user = _context.Users.FirstOrDefault(u => u.Phone == phone);
    if (phone.Length != 11)
    {
        TempData["error"] = "شماره تلفن وارد شده نادرست است";
        return RedirectToAction("Login");
    }
    else if (user == null)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                //create new user
                User us = new User
                {
                    Phone = phone,
                    FirstAndLastName = name,
                    Email = password,
                    Url = url,
                    Code = 0,
                    Cart = "0",
                    Adress = Adress,
                    Latitude = latitude,
                    Longitude = longitude,
                    free = 5,
                   
                    
                };
                _context.Users.Add(us);
                _context.SaveChanges();

                //find final id
                var quser = _context.Users.FirstOrDefault(u => u.Phone == phone);

                //add Device
                Device device = new Device
                {
                    DeviceId = dev,
                    UserId = quser.Id,
                    state = true,
                    
                };
                _context.Devices.Add(device);
                _context.SaveChanges();

                transaction.Commit();
                TempData["success"] = "ثبت نام با موفقیت انجام شد";
                return RedirectToAction("login", "phone");
            }
            catch (Exception)
            {
                transaction.Rollback();
                TempData["error"] = "ثبت نام با خطا مواجه شد";
                return RedirectToAction("Login");
            }
        }
    }
    else
    {
        TempData["error"] = "شماره تلفن وارد شده قبلا ثبت شده است";
        return RedirectToAction("Login");
    }

    

}




    //login


    public async Task<IActionResult> LoginmeAsync(string phone, string password, string dev2)
    {




        var quser = _context.Users.FirstOrDefault(u => u.Phone == phone && u.Email == password);

       

        




        if (quser != null)
        {
             //check quser.id and dev2 in device table
        var qdevice = _context.Devices.FirstOrDefault(u => u.UserId == quser.Id);
        if (qdevice == null)
        {
            Device device = new Device
            {
                DeviceId = dev2,
                UserId = quser.Id,
                state = true,
            };
            _context.Devices.Add(device);
            _context.SaveChanges();
        }
        // else if (qdevice.DeviceId != dev2)
        // {
        //     //tempdata
        //     TempData["error"] = "این شماره تلفن در دستگاه دیگری استفاده می شود";
        //     return RedirectToAction("login");
        // }
        

            /////////////////////////////////////////

          ClaimsIdentity identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, quser.FirstAndLastName),
                    new Claim(ClaimTypes.NameIdentifier, quser.Id.ToString()),
                    //add new claim for add value quser.cart if null set 0
                    new Claim("cart", quser.Cart.ToString() == "0" ? "0" : quser.Cart.ToString()),
                    new Claim("walet", dbWalet.ShowMojodi(phone).ToString() == null ? "0" : dbWalet.ShowMojodi(phone).ToString()),
                    new Claim("vige", quser.Url.ToString() == null ? "0" : quser.Url.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddYears(100), // Set the ExpiresUtc to a far-future date
                IsPersistent = true
            };
            await HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("home", "home");





        }
        else
        {
            //tempdata
            TempData["error"] = "شماره موبایل یا رمز عبور اشتباه است";
            return RedirectToAction("login");
        }


    }





//check code
  







    public async Task<IActionResult> BaresiAsync(int code, string deviceid)
    {
        var phone = HttpContext.Session.GetString("phone").ToString();

        var quser = dbuser.check(phone, code);
        // if quser.Cart is null then user is not exist
        if (quser.Cart == null)
        {
            quser.Cart = "0";
        }


        if (quser != null)
        {

            // var qdevice=dbuser.checkdevice(deviceid);
            // if (qdevice==false)
            // {
            //     dbuser.AddDevice(deviceid, quser.Id);
            // }else
            // {

            //     dbuser.deactivedevice(deviceid);

            // }

            // auttocation///////////////////////////////////////////

         
          ClaimsIdentity identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, quser.FirstAndLastName),
                    new Claim(ClaimTypes.NameIdentifier, quser.Id.ToString()),
                    //add new claim for add value quser.cart if null set 0
                    new Claim("cart", quser.Cart.ToString() == "0" ? "0" : quser.Cart.ToString()),
                    new Claim("walet", dbWalet.ShowMojodi(phone).ToString() == null ? "0" : dbWalet.ShowMojodi(phone).ToString()),
                    new Claim("vige", quser.Url.ToString() == null ? "0" : quser.Url.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddYears(100), // Set the ExpiresUtc to a far-future date
                IsPersistent = true
            };
            await HttpContext.SignInAsync(principal, properties);
            return RedirectToAction("home", "home");




        }
        else
        {
            return RedirectToAction("login");
        }


    }

    public IActionResult logout()
    {

        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //set state false Device


        return RedirectToAction("login", "phone", new { status = "disable" });
    }

    public IActionResult checkdivace(string deviceid)
    {
        var qdevice = dbuser.checkdevice(deviceid);
        if (qdevice == false)
        {
            return Json("faild");
        }
        else
        {
            //find userid by deviceid
            var q = dbuser.ShowUserByDevice(deviceid);

            ClaimsIdentity identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name ,q.FirstAndLastName ) ,
                            new Claim(ClaimTypes.NameIdentifier,q.Id.ToString() )
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

            var princpal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties

            {
                //expire by one weak
                ExpiresUtc = null,
                IsPersistent = true


            };
            HttpContext.SignInAsync(princpal, properties);

            return Json("success");

        }
    }


    //deactive device
    public void deactivedevice(string deviceid)
    {
        dbuser.deactivedevice(deviceid);

    }


    public IActionResult rule()
    {
        // TODO: Your code here
        return View();
    }



}