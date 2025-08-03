using BaseSource.ApiIntegration.WebApi;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared.Account;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.WebApp.Areas.Dashboard.Controllers
{
    public class AccountController : BaseDashboardController
    {
        private readonly IUserApiClient _userApiClient;
        public AccountController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Manage()
        {
            var result = await _userApiClient.GetUserInfo();
            return View(result.ResultObj);
        }

        public async Task<IActionResult> EditProfile()
        {
            var result = await _userApiClient.GetUserInfo();
            var model = new EditProfileVm();
            model.FirstName = result.ResultObj.FirstName;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVm model)
        {
            var result = await _userApiClient.EditProfile(model);
            if (result.IsSuccessed)
            {
                return RedirectToAction("Manage");
            }
            ModelState.AddListErrors(result.ValidationErrors);
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
        {
            var result = await _userApiClient.ChangePassword(model);
            if (result.IsSuccessed)
            {
                return RedirectToAction("Manage");
            }

            ModelState.AddListErrors(result.ValidationErrors);
            return View(model);
        }

        #region 2fa
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var result = await _userApiClient.EnableTwoFactorAuthentication();
            if (!result.IsSuccessed)
            {
                return NotFound();
            }

            var model = new EnableTwoFactorAuthenticationVm()
            {
                BarcodeUrl = result.ResultObj.BarcodeUrl,
                SecretKey = result.ResultObj.SecretKey
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userApiClient.EnableTwoFactorAuthentication(model.Code);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("Code", result.Message);
                return View(model);
            }

            return RedirectToAction("Manage", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var result = await _userApiClient.DisableTwoFactorAuthentication();
            if (!result.IsSuccessed)
            {
                return NotFound();
            }

            return RedirectToAction("Manage", "Account");
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> ResetApiKey()
        {
            var result = await _userApiClient.ResetApiKey();
            if (!result.IsSuccessed)
            {
                return Json(result);
            }
            return Json(new ApiSuccessResult<string>(Url.Action("Manage")));
        }
    }
}
