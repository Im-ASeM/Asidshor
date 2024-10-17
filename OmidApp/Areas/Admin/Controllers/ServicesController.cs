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

[Area("Admin")]
public class ServicesController : Controller
{

  
   private readonly Context _context;

    public ServicesController(Context _context)
    {
        
       
        this._context = _context;
    }
    
 

  public IActionResult Index()
    {
      

      //if services is null
      if(_context.Services.Count() == 0)
      {
        _context.Services.Add(new Service { Srvicename = "موتور", Parentid = 0, Status = "فعال" });
        _context.Services.Add(new Service { Srvicename = "گیربکس", Parentid = 0, Status = "فعال" });
        
      }

      _context.SaveChanges();
      ViewBag.Services = _context.Services.Where(x=>x.Parentid==0).ToList();
     
        

        return View();
    }

     public IActionResult service(int id ,string? meesage)
    {

      //session
      HttpContext.Session.SetInt32("id",id);
     
      //viewbad listcars
      ViewBag.listservice=_context.Services.Where(a=>a.Parentid==id).ToList();
      //viewbad catname
      ViewBag.catname=_context.Services.Find(id).Srvicename;
      //viewbad id
      ViewBag.id=id;
      //viewbad meesage
      ViewBag.meesage=meesage;
      return View();
    }


    //add service
   
    public IActionResult addserviceme(string Servicename,int id)
    {
     
      // //exist
      // if(_context.Services.Any(a=>a.Srvicename==Servicename))
      // {
      //   return RedirectToAction("service",new{id=id,meesage="این خدمت قبلا ثبت شده است"});
      // }
      //add
      _context.Services.Add(new Service { Srvicename = Servicename, Parentid =Convert.ToInt32(HttpContext.Session.GetInt32("id")) , Status = "فعال" });
      _context.SaveChanges();
      return RedirectToAction("service",new{id=id,meesage="خدمت جدید با موفقیت ثبت شد"});
     
    }

    //servicedelete
    public IActionResult servicedelete(int id)
    {
      //delete
      _context.Services.Remove(_context.Services.Find(id));
      _context.SaveChanges();
      return RedirectToAction("service",new{id=Convert.ToInt32(HttpContext.Session.GetInt32("id")),meesage="خدمت با موفقیت حذف شد"});
    }
    

}


