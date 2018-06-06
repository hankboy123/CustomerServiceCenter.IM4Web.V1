using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace CustomerServiceCenter.IM4Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            
        }

        protected virtual string CurrentUserId
        {
            get { return ""; }
        }
        protected virtual string CurrentLoginId
        {
            get { return "";  }
        }
    }
}