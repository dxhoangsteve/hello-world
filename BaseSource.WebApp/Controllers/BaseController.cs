using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static BaseSource.Shared.Constants.SystemConstants;

namespace BaseSource.WebApp.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private string _userId;
        public string UserId
        {
            get { return _userId ?? User.FindFirstValue(ClaimTypes.NameIdentifier); }
            set { _userId = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var token = Request.Cookies[AppSettings.Token];
            //if (token == null)
            //{
            //    if (User.Identity.IsAuthenticated)
            //    {
            //        context.Result = new RedirectResult(Url.Action("Login", "Account", new { returnUrl = Request.Path }));
            //    }
            //}
            //else if (!User.Identity.IsAuthenticated)
            //{
            //    Response.Cookies.Append(SystemConstants.AppSettings.Token, "", new CookieOptions()
            //    {
            //        Expires = DateTime.Now.AddYears(-1)
            //    });
            //}
            base.OnActionExecuting(context);
        }
    }
}
