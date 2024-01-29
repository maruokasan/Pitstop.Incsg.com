using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization; // Add this line

namespace Pitstop.Helper
{
    public class Authorize : ActionFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Check if the action or controller has [AllowAnonymous] attribute
            if (filterContext.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            {
                return; // Allow anonymous access
            }

            if (string.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies[AppConstant.CookiesName.UserId]))
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary { { "controller", "Auth" }, { "Action", "AccessDenied" } });
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies[AppConstant.CookiesName.RolePermission]))
                {
                    return;
                }

                var rolePermission = JsonConvert.DeserializeObject<List<RolePermission>>(filterContext.HttpContext.Request.Cookies[AppConstant.CookiesName.RolePermission]);
                var roleAccess = rolePermission.Where(data => data.ClaimType.ToUpper() == ClaimType.ToUpper() && ClaimValue.ToUpper().Contains(data.ClaimValue.ToUpper())).FirstOrDefault();
                if (roleAccess == null)
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "controller", "Auth" }, { "Action", "AccessDenied" } });
                    return;
                }
            }
        }
    }
}
