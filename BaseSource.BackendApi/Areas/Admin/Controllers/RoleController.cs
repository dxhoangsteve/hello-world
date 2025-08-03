using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Admin;
using X.PagedList;
using Microsoft.AspNetCore.Identity;
using BaseSource.PermissionBased.ViewModels;
using BaseSource.Shared.Constants;
using BaseSource.PermissionBased.Constants;
using BaseSource.PermissionBased.Helpers;
using System.Security.Claims;
using System.Security;
using Microsoft.AspNetCore.Authorization;

namespace BaseSource.BackendApi.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class RoleController : BaseAdminApiController
    {
        private readonly BaseSourceDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleController(BaseSourceDbContext context,
                UserManager<AppUser> userManager,
                RoleManager<AppRole> roleManager
            )
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("GetAlls")]
        public async Task<IActionResult> GetAlls()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

            return Ok(new ApiSuccessResult<List<RoleVm>>(roles));
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            var model = new RoleVm()
            {
                Id = existingRole.Id,
                Name = existingRole.Name,
                Description = existingRole.Description
            };

            return Ok(new ApiSuccessResult<RoleVm>(model));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateRoleVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var existingRole = await _roleManager.FindByNameAsync(model.Name);
            if (existingRole != null)
            {
                ModelState.AddModelError(nameof(model.Name), "Similar Role already exists.");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var role = new AppRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Description = model.Description
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(new ApiSuccessResult<string>());
            }

            AddErrors(result, nameof(model.Name));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EditRoleVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var existingRole = await _roleManager.FindByIdAsync(model.Id);
            if (existingRole == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            if (RoleConstants.NotAllowedModify.Contains(existingRole.Name))
            {
                ModelState.AddModelError("Name", $"Not allowed to modify {existingRole.Name} Role.");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            existingRole.Name = model.Name;
            existingRole.Description = model.Description;

            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
            {
                return Ok(new ApiSuccessResult<string>());
            }

            AddErrors(result, nameof(model.Name));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromForm] string id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            if (RoleConstants.NotAllowedModify.Contains(existingRole.Name))
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            if ((await _userManager.GetUsersInRoleAsync(existingRole.Name)).Count > 0)
            {
                return Ok(new ApiErrorResult<string>($"Not allowed to delete {existingRole.Name} Role as it is being used."));
            }

            var result = await _roleManager.DeleteAsync(existingRole);
            if (result.Succeeded)
            {
                return Ok(new ApiSuccessResult<string>());
            }

            return Ok(new ApiErrorResult<string>(string.Join(". ", result.Errors)));
        }

        #region permission
        [HttpGet("GetPermissionsByRoleId")]
        public async Task<IActionResult> GetPermissionsByRoleId(string roleId)
        {
            var model = new PermissionVm();
            var allPermissions = ClaimsHelper.GetAllPermissions();

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                model.RoleId = role.Id;
                model.RoleName = role.Name;

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var permission in allPermissions)
                {
                    if (claims.Any(x => x.Value == permission.Value))
                    {
                        permission.Selected = true;
                    }
                }
            }
            model.RoleClaims = allPermissions;

            return Ok(new ApiSuccessResult<PermissionVm>(model));
        }

        [HttpPost("UpdatePermission")]
        public async Task<IActionResult> UpdatePermission(PermissionVm model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (RoleConstants.NotAllowedModify.Contains(role.Name))
            {
                return Ok(new ApiErrorResult<string>($"Not allowed to modify Permissions for this Role."));
            }
            var claims = await _roleManager.GetClaimsAsync(role);
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();

            foreach (var claim in claims)
            {
                if (!selectedClaims.Any(x => x.Value == claim.Value))
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
            }

            foreach (var claim in selectedClaims.Where(x => !claims.Any(y => y.Value == x.Value)))
            {
                await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, claim.Value));
            }

            return Ok(new ApiSuccessResult<string>());
        }
        #endregion

        #region helper
        private void AddErrors(IdentityResult result, string Property)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(Property, error.Description);
                break;
            }
        }
        #endregion
    }
}
