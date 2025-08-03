using BaseSource.ApiIntegration;
using BaseSource.ApiIntegration.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.AdminApp.Areas.Admin.ViewComponents
{
    public class UserInfoTopBarViewComponent : ViewComponent
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoTopBarViewComponent(IUserApiClient userApiClient, IHttpContextAccessor httpContextAccessor)
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
