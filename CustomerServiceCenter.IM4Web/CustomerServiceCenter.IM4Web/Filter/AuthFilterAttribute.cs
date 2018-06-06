using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using RestSharp;
using Exceptionless;

namespace CustomerServiceCenter.IM4Web.Filter
{
    public class AuthFilterAttribute : Attribute,  IActionFilter
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(AuthFilterAttribute));

        IRestClient passportRestClient = new RestClient("https://passport.5173.com/Passport/");

        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAuthPass = false;
            ResultValidateDTO result = null;
            try
            {
                //passportRestClient.PreAuthenticate
                result = ValidateToken(getToken(getAuthCookie(context)));
                if (result != null)
                {
                    if (string.Equals(result.ResultNo, "0"))
                    {
                        isAuthPass = true;
                    }
                    else
                    {
                        throw new Exception("OnActionExecuting|result.ResultNo：" + result.ResultNo + "|result.ResultDescription：" + result.ResultDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "" + ex.StackTrace);
                ex.ToExceptionless().Submit();
                //ExceptionlessClient.Default.CreateLog(typeof(OrderService).FullName, ex.ToString(), LogLevel.Error.ToString()).AddTags("Exception").Submit();
            }



            if (!isAuthPass)
            {
                string returnUrl = "http://" + context.HttpContext.Request.Host.ToString();
                string strLoginUrl = "https://passport.5173.com/?returnUrl=" + HttpUtility.UrlEncode(returnUrl); //登录直接掉转passport
                                                                                                                                                                                      ///TODO:g过滤返回请求头内容
                context.HttpContext.Response.Redirect(strLoginUrl);

            }
            else
            {
                context.HttpContext.Session.SetString("userid", result.Ticket.UserID);
                // context.HttpContext.User.Identity.Name = result.Ticket.UserName;
            }

            // do something before the action executes
        }

        private string getAuthCookie(ActionExecutingContext context,string authCookieKey= ".5173auth")
        {
            IRequestCookieCollection cookies = context.HttpContext.Request.Cookies;
            string auth5173Value = string.Empty;
            if (cookies != null && cookies.Count > 0)
            {
                if (cookies.ContainsKey(authCookieKey))
                {
                    if (!cookies.TryGetValue(authCookieKey, out auth5173Value))
                    {
                        new Exception("没有 “5173auth” cookie").ToExceptionless().Submit();
                    }
                }
            }
            return auth5173Value;
        }


        private string getToken(string cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie)) return string.Empty;

            var request = new RestRequest("ValidateCookie", Method.GET);
            request.AddParameter("value", cookie); //

            // execute the request
            IRestResponse response = passportRestClient.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception("getToken|ErrorMessage:" + response.ErrorMessage+ ",ErrorException:" + response.ErrorException);
            }


            string content = response.Content; // raw content as string

            ResultValidateDTO result= JsonConvert.DeserializeObject<ResultValidateDTO>(content);

            if (result!=null)
            {
                if (string.Equals(result.ResultNo, "0"))
                {
                    if (result.Ticket != null)
                    {
                        return result.Ticket.Token;
                    }
                }
                else
                {
                    throw new Exception("getToken|result.ResultNo：" + result.ResultNo+ "|result.ResultDescription：" + result.ResultDescription);
                }
                
            }
            
            return string.Empty;
        }

        private ResultValidateDTO ValidateToken(string token,string appno= "passport")
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var request = new RestRequest("ValidateToken", Method.GET);
            request.AddParameter("token", token);
            request.AddParameter("appNo", appno);

            // execute the request
            IRestResponse response = passportRestClient.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception("ValidateToken|ErrorMessage:" + response.ErrorMessage + ",ErrorException:" + response.ErrorException);
            }
            string content = response.Content; //

            ResultValidateDTO result = JsonConvert.DeserializeObject<ResultValidateDTO>(content);
            

            return result;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
        

        public class ResultValidateDTO
        {
            public string ResultNo;
            public string ResultDescription;
            public Ticket Ticket
            {
                get;
                set;
            }
            public bool IsValidate()
            {
                return ResultNo.Equals("0");
            }
        }
        public class Ticket
        {
            public string Token
            {
                get;
                set;
            }
            public string UserID
            {
                get; set;
            }

            public string UserName
            {
                get;
                set;
            }
        }

    }
}
