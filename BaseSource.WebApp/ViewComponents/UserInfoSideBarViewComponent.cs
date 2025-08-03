using BaseSource.ApiIntegration.WebApi;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static BaseSource.Shared.Constants.SystemConstants;

namespace BaseSource.WebApp.ViewComponents
{
    [Authorize]
    public class UserInfoSideBarViewComponent : ViewComponent
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoSideBarViewComponent(IUserApiClient userApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _userApiClient = userApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _userApiClient.GetUserInfo();
            return View("Default", result.ResultObj);
        }
    }
}
