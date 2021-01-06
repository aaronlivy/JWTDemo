using AntifogeryDemo.Filters;
using AntifogeryDemo.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AntifogeryDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]      
        [AjaxValidateAntiForgeryTokenAttribute]
        public ActionResult AjaxEdit()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AntiforgeryView()
        {
            return View();
        }

        [HttpPost]
        [AjaxValidateTokenAttribute]
        public ActionResult AjaxTokenDemo()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAntiforgeryToken()
        {
            string cookieToken, formToken;
            string result = string.Empty;

            if(Session.Count != 0 &&  !string.IsNullOrEmpty(Session["__RequestVerificationToken"].ToString()))
            {
                result = Session["__RequestVerificationToken"].ToString();
            }
            else
            {
                AntiForgery.GetTokens(null, out cookieToken, out formToken);
                result = cookieToken + ":" + formToken;
                Session["__RequestVerificationToken"] = result;
            }
                 
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToken()
        {
            string formToken = string.Empty;
            formToken = GenerateToken();
            return Json(formToken, JsonRequestBehavior.AllowGet);
        }

        public string GenerateToken()
        {
            string result = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            char[] chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int length = RNG.Next(64, 64);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[RNG.Next(chars.Length - 1)]);
            }

            result = sb.ToString();

            TokenCache.SetToken(result);

            return result;
        }
    }
}