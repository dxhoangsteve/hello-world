using BaseSource.ApiIntegration.AdminApi;
using BaseSource.PermissionBased.ViewModels;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.AdminApp.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class RoleController : BaseAdminController
    {
        private readonly IRoleAdminApiClient _apiClient;
        public RoleController(IRoleAdminApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _apiClient.GetAlls();
            return View(data.ResultObj);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _apiClient.Create(model);
            if (!result.IsSuccessed)
            {
                return Json(result);
            }
            return Json(new ApiSuccessResult<string>(Url.Action("Index")));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var model = await _apiClient.GetById(id);
            if (!model.IsSuccessed)
            {
                return NotFound();
            }
            var data = new EditRoleVm();
            data.Id = id;
            data.Name = model.ResultObj.Name;
            data.Description = model.ResultObj.Description;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _apiClient.Edit(model);
            if (!result.IsSuccessed)
            {
                return Json(result);
            }
            return Json(new ApiSuccessResult<string>(Url.Action("Index")));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _apiClient.Delete(id);
            if (result.IsSuccessed)
            {
                return Json(new ApiSuccessResult<string>());
            }
            return Json(new ApiErrorResult<string>(result.Message));
        }

        public async Task<IActionResult> Permissions(string roleId)
        {
            var model = await _apiClient.GetPermissionsByRoleId(roleId);
            if (!model.IsSuccessed)
            {
                return NotFound();
            }

            return View(model.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermission(PermissionVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _apiClient.UpdatePermission(model);
            if (!result.IsSuccessed)
            {
                return Json(result);
            }
            return Json(new ApiSuccessResult<string>(Url.Action("Index")));
        }
    }
}
