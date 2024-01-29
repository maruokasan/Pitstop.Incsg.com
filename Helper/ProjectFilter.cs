using Pitstop.Models;
using Pitstop.Models.PitstopData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Pitstop.Helper
{
    public class ProjectFilter : ActionFilterAttribute
    {
        private readonly PitstopContext _PitstopContext;
        private readonly UserManager<User> _userManager;
        public ProjectFilter(PitstopContext PitstopContext, UserManager<User> userManager)
        {
            _PitstopContext = PitstopContext;
            _userManager = userManager;
        }

         public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var path = filterContext.HttpContext.Request.Path;

            List<string> allowedPaths = new List<string>
            {
                "/Home/Cart",
            };

            if (allowedPaths.Any(allowedPath => path.Value.Contains(allowedPath)))
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            
            if (path.Value.Contains("Auth/Login") || path.Value.Contains("Auth/Index") || path.Value == "/")
            {
                return;
            }

            try
            {
                var userId = _userManager.GetUserId(filterContext.HttpContext.User);
                if (userId != null)
                {
                    var user = _PitstopContext.Users.Find(userId);

                    user.LastAccessDate = DateTime.Now;
                    _PitstopContext.Users.Update(user);

                    try
                    {
                        _PitstopContext.SaveChanges();
                    }
                    catch
                    {
                    }


                    List<string> roleId = _PitstopContext.UserRoles.Where(d => d.UserId.Contains(user.Id)).Select(d => d.RoleId).ToList();
                    var rolePermission = _PitstopContext.RolePermissions.Where(d => roleId.Contains(d.RoleId)).ToList();

                    //Development//
                    filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.UserId, user.Id);
                    filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.Username, user.UserName);
                    filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.RolePermission, JsonConvert.SerializeObject(rolePermission));

                    //Live//
                    // filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.UserId, user.Id, new CookieOptions { Domain = AppConstant.CookiesName.domain });
                    // filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.Username, user.UserName, new CookieOptions { Domain = AppConstant.CookiesName.domain });
                    // filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.RolePermission, JsonConvert.SerializeObject(rolePermission), new CookieOptions { Domain = AppConstant.CookiesName.domain });

                                    }
                else if (!path.Value.Contains("Auth/ForgotPassword") && !path.Value.Contains("Auth/ResetMyPassword"))
                {
                    ReturnToLogin(filterContext);
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
            catch
            {
                ReturnToLogin(filterContext);
                base.OnActionExecuting(filterContext);
            }

        }

        public void ReturnToLogin(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.UserId, String.Empty);
            filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.Username, String.Empty);
            filterContext.HttpContext.Response.Cookies.Append(AppConstant.CookiesName.RolePermission, String.Empty);

            filterContext.Result = new RedirectResult("~/");
        }
    }
}