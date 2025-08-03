using BaseSource.ApiIntegration.AdminApi;
using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.AdminApp.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class SettingController : BaseAdminController
    {
        private readonly ISettingAdminApiClient _apiClient;
        public SettingController(ISettingAdminApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _apiClient.GetAlls();
            if (!result.IsSuccessed)
            {
                return NotFound();
            }

            return View(result.ResultObj);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ConfigViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _apiClient.UpdateConfig(model);
            if (!result.IsSuccessed)
            {
                ModelState.AddListErrors(result.ValidationErrors);
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
