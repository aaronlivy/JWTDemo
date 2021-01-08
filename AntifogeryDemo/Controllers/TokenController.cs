using AntifogeryDemo.Methods;
using AntifogeryDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using AntifogeryDemo.Filters;

namespace AntifogeryDemo.Controllers
{
    public class TokenController : Controller
    {           

        //登入
        [HttpPost]
        public ActionResult SignIn(string Username, string Password)
        {
            //模擬從資料庫取得資料
            if (!(Username == "demo" && Password == "one23"))
            {
                throw new Exception("登入失敗，帳號或密碼錯誤");
            }

            var user = new User
            {
                CustId = "00001",
                Name = "小明",
                LoginDateTime = DateTime.Now
            };

            //產生 Token
            var token = TokenManager.Create(user);
            //需存入資料庫
            TokenCache.SetToken(token.access_token);
            return Json(token, JsonRequestBehavior.AllowGet);
        }

        //換取新 Token
        [HttpPost]
        public ActionResult Refresh(string refreshToken)
        {
            //檢查 Refresh Token 是否正確
            if (!TokenManager.ContainsKey(refreshToken))
            {
                throw new Exception("查無此 Refresh Token");
            }
            //需查詢資料庫
            var user = TokenManager.GetToken(refreshToken);
            //產生一組新的 Token 和 Refresh Token
            var token = TokenManager.Create(user);
            //刪除舊的
            TokenManager.Remove(refreshToken);
            //存入新的
            TokenManager.Add(token.refresh_token, user);
            return Json(token, JsonRequestBehavior.AllowGet);
        }

        //測試是否通過驗證
        [HttpPost]
        [JWTValidate]
        public ActionResult IsAuthenticated()
        {
            var result = true;
            
            var token = TokenManager.Create(result);

            return Json(token, JsonRequestBehavior.AllowGet);
        }
    }
}
