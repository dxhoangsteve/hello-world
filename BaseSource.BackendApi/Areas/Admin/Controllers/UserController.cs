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
using BaseSource.Shared.Enums;
using BaseSource.PermissionBased.Constants;
using Microsoft.AspNetCore.Authorization;
using BaseSource.PermissionBased.ViewModels;
using System.Data;

namespace BaseSource.BackendApi.Areas.Admin.Controllers
{
    public class UserController : BaseAdminApiController
    {
        private readonly BaseSourceDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(BaseSourceDbContext context,
                UserManager<AppUser> userManager,
                RoleManager<AppRole> roleManager
            )
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Policy = Permissions.User.View)]
        [HttpGet("GetPagings")]
        public async Task<IActionResult> GetPagings([FromQuery] GetUserPagingRequest_Admin request)
        {
            var query = (from u in _db.Users
                         //join ur in _db.UserRoles on u.Id equals ur.UserId into ur_join
                         //from ur in ur_join.DefaultIfEmpty()
                         join profile in _db.UserProfiles on u.Id equals profile.UserId
                         //where !string.IsNullOrEmpty(request.RoleId) ? ur.RoleId == request.RoleId : true
                         select new UserVm()
                         {
                             Id = u.Id,
                             CustomId = profile.CustomId,
                             UserName = u.UserName,
                             Email = u.Email,
                             Roles = (from ur in _db.UserRoles
                                      join r in _db.Roles on ur.RoleId equals r.Id
                                      where ur.UserId == u.Id
                                      select r.Name).ToList(),
                             JoinedDate = profile.JoinedDate,
                             LockoutEnd = u.LockoutEnd,
                             Phone = u.PhoneNumber,
                             EmailConfirmed = u.EmailConfirmed,
                             FirstName = profile.FirstName,
                             LastName = profile.LastName,
                         });

            if (!string.IsNullOrEmpty(request.RoleId))
            {
                var role = await _db.Roles.FindAsync(request.RoleId);
                if (request != null)
                {
                    query = query.Where(x => x.Roles.Any(x => x == role.Name));
                }
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.UserName.Contains(request.UserName));
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                query = query.Where(x => x.Email.Contains(request.Email));
            }

            if (request.Locked == true)
            {
                query = query.Where(x => x.LockoutEnd != null);
            }
            else
            {
                query = query.Where(x => x.LockoutEnd == null);
            }

            var data = await query.OrderByDescending(x => x.JoinedDate)
                .ToPagedListAsync(request.Page, request.PageSize);

            var pagedResult = new PagedResult<UserVm>()
            {
                TotalItemCount = data.TotalItemCount,
                PageSize = data.PageSize,
                PageNumber = data.PageNumber,
                Items = data.ToList()
            };

            return Ok(new ApiSuccessResult<PagedResult<UserVm>>(pagedResult));
        }

        [Authorize(Policy = Permissions.User.View)]
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult> GetAllRoles()
        {
            var allRoles = await _roleManager.Roles.OrderBy(x => x.Name)
                .Select(x => new SelectItem
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return Ok(new ApiSuccessResult<List<SelectItem>>(allRoles));
        }

        [Authorize(Policy = Permissions.User.Role)]
        [HttpGet("GetUserRoles")]
        public async Task<ActionResult> GetUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>());
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

            var roleAssignRequest = new RoleAssignVm();
            roleAssignRequest.Id = id;
            foreach (var role in allRoles.OrderBy(x => x))
            {
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role,
                    Name = role,
                    Selected = userRoles.Contains(role)
                });
            }

            return Ok(new ApiSuccessResult<RoleAssignVm>(roleAssignRequest));
        }

        [Authorize(Policy = Permissions.User.Role)]
        [HttpPost("RoleAssign")]
        public async Task<ActionResult> RoleAssign(RoleAssignVm model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return Ok(new ApiErrorResult<string>());
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                var addedRoles = model.Roles.Where(x => x.Selected).Select(x => x.Name).ToList();
                if (model.Roles != null)
                {
                    await _userManager.AddToRolesAsync(user, addedRoles);
                }
                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>());
        }

        [Authorize(Policy = Permissions.User.ResetPassword)]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromForm] string id)
        {
            var newPassword = Guid.NewGuid().ToString("n").Substring(0, 8);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>("Not found!"));
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
            user.SecurityStamp = Guid.NewGuid().ToString("D");
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new ApiSuccessResult<string>(newPassword));
            }

            return Ok(new ApiErrorResult<string>("Error!"));
        }

        [Authorize(Policy = Permissions.User.Lock)]
        [HttpPost("Lock")]
        public async Task<IActionResult> Lock([FromForm] string id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.LockoutEnabled = true;

                if (user.LockoutEnd == null)
                {
                    user.LockoutEnd = DateTime.Now.AddYears(200);
                }
                else
                {
                    user.LockoutEnd = null;
                }
                await _db.SaveChangesAsync();

                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>());
        }

        [Authorize(Policy = Permissions.User.View)]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string userId)
        {
            var user = await _db.Users
                .Include(x => x.UserProfile)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            var result = new UserAdminInfoDto
            {
                Id = userId,
                CustomId = user.UserProfile.CustomId,
                Email = user.Email,
                FirstName = user.UserProfile.FirstName,
                JoinedDate = user.UserProfile.JoinedDate,
                LastName = user.UserProfile.LastName,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                LockoutEndDateUtc = user.LockoutEnd != null ? user.LockoutEnd.Value.UtcDateTime : null,
                EmailConfirmed = user.EmailConfirmed,
                PasswordText = user.PasswordText,
            };

            return Ok(new ApiSuccessResult<UserAdminInfoDto>(result));
        }

        [Authorize(Policy = Permissions.User.Edit)]
        [HttpPost]
        [Route("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(UserVerifyDto model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }
            user.EmailConfirmed = true;
            await _db.SaveChangesAsync();

            return Ok(new ApiSuccessResult<string>());
        }

        [Authorize(Policy = Permissions.User.Create)]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateUserAdminVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            if (!string.IsNullOrEmpty(model.CustomId))
            {
                if (await _db.UserProfiles.AnyAsync(x => x.CustomId == model.CustomId))
                {
                    ModelState.AddModelError("CustomId", "Mã NV đã tồn tại");
                    return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
                }
            }

            if (await _db.Users.AnyAsync(x => x.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "UserName already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            if (!string.IsNullOrEmpty(model.Email) && await _db.Users.AnyAsync(x => x.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber) && await _db.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var newUser = new AppUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = !string.IsNullOrEmpty(model.Email),
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber),
                PasswordText = model.Password
            };
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                _db.UserProfiles.Add(new UserProfile
                {
                    UserId = newUser.Id,
                    CustomId = !string.IsNullOrEmpty(model.CustomId) ? model.CustomId : newUser.Id.ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    JoinedDate = DateTime.Now,
                    ApiKey = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"),
                });

                await _db.SaveChangesAsync();
                return Ok(new ApiSuccessResult<string>());
            }
            AddErrors(result, nameof(model.UserName));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
        }

        [Authorize(Policy = Permissions.User.Edit)]
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(EditUserAdminVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var user = await _db.UserProfiles.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.UserId == model.Id);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>("Not found"));
            }

            if (!string.IsNullOrEmpty(model.CustomId))
            {
                if (await _db.UserProfiles.AnyAsync(x => x.CustomId == model.CustomId && x.UserId != model.Id))
                {
                    ModelState.AddModelError("CustomId", "Mã NV đã tồn tại");
                    return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
                }
            }

            if (await _db.Users.AnyAsync(x => x.UserName == model.UserName && x.Id != model.Id))
            {
                ModelState.AddModelError("UserName", "UserName already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            if (!string.IsNullOrEmpty(model.Email) && await _db.Users.AnyAsync(x => x.Email == model.Email && x.Id != model.Id))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            if (!string.IsNullOrEmpty(model.PhoneNumber) && await _db.Users.AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            user.CustomId = !string.IsNullOrEmpty(model.CustomId) ? model.CustomId : Guid.NewGuid().ToString();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.AppUser.UserName = model.UserName;
            user.AppUser.NormalizedUserName = model.UserName?.ToUpper();
            user.AppUser.Email = model.Email;
            user.AppUser.NormalizedEmail = model.Email?.ToUpper();
            user.AppUser.EmailConfirmed = !string.IsNullOrEmpty(model.Email);
            user.AppUser.PhoneNumber = model.PhoneNumber;
            user.AppUser.PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber);

            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user.AppUser);
                await _userManager.ResetPasswordAsync(user.AppUser, token, model.Password);

                user.AppUser.PasswordText = model.Password;
            }

            await _db.SaveChangesAsync();

            return Ok(new ApiSuccessResult<string>());
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUserRequest_Admin request)
        {
            var query = (from u in _db.Users
                         join profile in _db.UserProfiles on u.Id equals profile.UserId
                         where u.LockoutEnd == null
                         select new UserVm()
                         {
                             Id = u.Id,
                             CustomId = profile.CustomId,
                             UserName = u.UserName,
                             Email = u.Email,
                             Roles = (  from ur in _db.UserRoles
                                        join rc in _db.RoleClaims on ur.RoleId equals rc.RoleId
                                        where ur.UserId == u.Id
                                        select rc.ClaimValue).ToList(),
                             FirstName = profile.FirstName,
                             LastName = profile.LastName,
                         });

            var data = await query.Select(x => new SelectItem()
            {
                Id = x.Id,
                Name = (x.LastName != null ? x.LastName + " " : "") +  x.FirstName + " - " + x.CustomId
            })
            .OrderBy(x => x.Name)
            .ToListAsync();

            return Ok(new ApiSuccessResult<List<SelectItem>>(data));
        }

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
