using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using CustomerServiceCenter.IM4Web.Models;
using RestSharp;
using static CustomerServiceCenter.IM4Web.Filter.AuthFilterAttribute;
using Newtonsoft.Json;
using CustomerServiceCenter.IM4Web.Filter;
using Microsoft.AspNetCore.Http;

namespace CustomerServiceCenter.IM4Web.Controllers
{
    
    public class HomeController : BaseController
    {
      
        public HomeController(IRestClient restClient)
        {
          
        }

        [AuthFilter]
        public IActionResult Index()
        {
            //this.HttpContext.Session.GetString("userid");
            return View();
        }

        public string F5()
        {
            //this.HttpContext.Session.GetString("userid");
            return "ok";
        }

    }
}
