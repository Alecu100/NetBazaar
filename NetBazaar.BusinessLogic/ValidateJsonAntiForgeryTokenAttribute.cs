using System;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NetBazaar.BusinessLogic
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ValidateJsonAntiForgeryTokenAttribute
        : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie != null ? cookie.Value : null,
                httpContext.Request.Headers["__RequestVerificationToken"]);
        }
    }
}