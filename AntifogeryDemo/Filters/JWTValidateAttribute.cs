using AntifogeryDemo.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AntifogeryDemo.Filters
{
    public class JWTValidateAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest() || !ValidateRequestHeader(filterContext.HttpContext.Request))
                {
                    filterContext.HttpContext.Response.StatusCode = 404;
                    filterContext.Result = new HttpNotFoundResult();
                }
            }
            catch (HttpAntiForgeryException e)
            {
                throw new HttpAntiForgeryException("Anti forgery token cookie not found");
            }
        }

        private bool ValidateRequestHeader(HttpRequestBase request)
        {
            string tokenValue = request.Headers["RequestVerificationToken"];

            var result = false;

            if (!string.IsNullOrEmpty(tokenValue))
            {
                result = TokenCache.GetToken(tokenValue);
            }

            return result;
        }
    }
}