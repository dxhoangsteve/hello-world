using BaseSource.ApiIntegration.AdminApi;
using BaseSource.PermissionBased.Constants;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BaseSource.PermissionBased.Constants.Permissions;

namespace BaseSource.AdminApp.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserAdminApiClient _apiClient;
        public UserController(IUserAdminApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [Authorize(Policy = Permissions.User.View)]
        public async Task<IActionResult> Index(string username, string email, bool? locked, string roleId, int? page = 1)
        {
            var request = new GetUserPagingRequest_Admin()
            {
                Page = page.Value,
                PageSize = 20,
                UserName = username,
                Email = email,
                Locked = locked,
                RoleId = roleId
            };

            var result = await _apiClient.GetPagings(request);
            if (!result.IsSuccessed)
            {
                return NotFound();
            }

            var resultRoles = await _apiClient.GetAllRoles();
            ViewBag.Roles = resultRoles.ResultObj.Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });
            return View(result.ResultObj);
        }

        [Authorize(Policy = Permissions.User.Role)]
        public async Task<ActionResult> EditUserRole(string id)
        {
            var result = await _apiClient.GetUserRoles(id);
            if (!result.IsSuccessed)
            {
                return NotFound();
            }

            return PartialView("_EditUserRole", result.ResultObj);
        }

        [Authorize(Policy = Permissions.User.Role)]
        [HttpPost]
        public async Task<ActionResult> EditUserRole(RoleAssignVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(false);
            }

            var result = await _apiClient.RoleAssign(model);
            if (!result.IsSuccessed)
            {
                return Json(false);
            }

            return Json(true);
        }

        [Authorize(Policy = Permissions.User.ResetPassword)]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var result = await _apiClient.ResetPassword(id);
            return Json(result);
        }

        [Authorize(Policy = Permissions.User.Lock)]
        [HttpPost]
        public async Task<IActionResult> Lock(string id)
        {
            var result = await _apiClient.Lock(id);
            if (result.IsSuccessed)
            {
                return Json(new ApiSuccessResult<string>());
            }
            return Json(new ApiErrorResult<string>(result.Message));
        }

        [Authorize(Policy = Permissions.User.Edit)]
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string id)
        {
            var result = await _apiClient.VerifyEmailAsync(new UserVerifyDto { UserId = id });
            if (result.IsSuccessed)
            {
                return Json(new ApiSuccessResult<string>());
            }
            return Json(new ApiErrorResult<string>(result.Message));
        }

        [Authorize(Policy = Permissions.User.View)]
        [HttpGet]
        public async Task<IActionResult> Info(string userId)
        {
            var result = await _apiClient.GetById(userId);
            if (result == null || result.ResultObj == null)
            {
                return NotFound();
            }

            return View(result.ResultObj);
        }

        [Authorize(Policy = Permissions.User.Edit)]
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserAdminVm userAdminInfoDto)
        {
            var result = await _apiClient.Edit(userAdminInfoDto);
            return Json(result);
        }

        [Authorize(Policy = Permissions.User.Create)]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [Authorize(Policy = Permissions.User.Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserAdminVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _apiClient.Create(model);
            if (!result.IsSuccessed)
            {
                return Json(new ApiErrorResult<string>(result.ValidationErrors));
            }
            return Json(new ApiSuccessResult<string>(Url.Action("Index")));
        }
    }
}
