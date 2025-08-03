using BaseSource.BackendApi.Services.Email;
using BaseSource.BackendApi.Services.Helper;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.PermissionBased.Constants;
using BaseSource.Shared.Constants;
using BaseSource.Shared.Enums;
using BaseSource.Utilities.Helper;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared.Account;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseSource.BackendApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly BaseSourceDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        private readonly ISendEmailService _emailService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountController(BaseSourceDbContext db, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, IConfiguration configuration, ISendEmailService emailService,
             IHttpContextAccessor httpContextAccessor,
             RoleManager<AppRole> roleManager)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;

        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            if (await _userManager.FindByNameAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already existss");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var appUser = new AppUser()
            {
                Email = model.Email,
                UserName = model.Email,
                PasswordText = model.Password
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (result.Succeeded)
            {
                var userDetail = new UserProfile()
                {
                    UserId = appUser.Id.ToString(),
                    FirstName = model.Email,
                    CustomId = appUser.Id.ToString(),
                    ApiKey = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"),
                };
                _db.UserProfiles.Add(userDetail);
                _db.SaveChanges();

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                //await _emailService.SendMailConfirmEmail(appUser.UserName, appUser.Email, appUser.Id, code);

                return Ok(new ApiSuccessResult<string>());
            }

            AddErrors(result, nameof(model.Email));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
        }
        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginRequestVm user)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var existingUser = await _userManager.FindByNameAsync(user.UserName) ?? await _userManager.FindByEmailAsync(user.UserName);
            if (existingUser == null)
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập hoặc mật khẩu không đúng");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            //if (!existingUser.EmailConfirmed)
            //{
            //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
            //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //    await _emailService.SendMailConfirmEmail(existingUser.UserName, existingUser.Email, existingUser.Id, code);

            //    ModelState.AddModelError("UserName", "Please verify your email before logging in.");
            //    return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            //}

            var validCredentials = await _userManager.CheckPasswordAsync(existingUser, user.Password);
            if (!validCredentials)
            {
                ModelState.AddModelError("UserName", "The username or password is incorrect.");
                return Ok(new ApiErrorResult<LoginResponseVm>(ModelState.GetListErrors()));
            }

            if (existingUser.LockoutEnabled && existingUser.LockoutEnd != null && existingUser.LockoutEnd > DateTime.Now)
            {
                ModelState.AddModelError("UserName", "User account locked out.");
                return Ok(new ApiErrorResult<LoginResponseVm>(ModelState.GetListErrors()));
            }

            if (existingUser.TwoFactorEnabled)
            {
                var obj = new Login2FaVerifyObj()
                {
                    UserId = existingUser.Id,
                    Exp = DateTime.Now.AddMinutes(15)
                };
                return Ok(new ApiSuccessResult<LoginResponseVm>(new LoginResponseVm()
                {
                    Token = await EncryptionHelper.EncryptAsync(JsonConvert.SerializeObject(obj)),
                    TwoFactorEnabled = existingUser.TwoFactorEnabled
                }));
            }
            else
            {
                var jwtToken = await GenerateJwtToken(existingUser);
                return Ok(new ApiSuccessResult<LoginResponseVm>(new LoginResponseVm()
                {
                    Token = jwtToken
                }));
            }
        }

        [HttpPost("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVm model)
        {

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return Ok(new ApiErrorResult<string>());
            }

            model.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            // Xác thực email
            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
            {
                return Ok(new ApiSuccessResult<string>());
            }

            AddErrors(result, nameof(model.Code));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            else
            {
                var tokenGenerated = await _userManager.GeneratePasswordResetTokenAsync(user);
                byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(tokenGenerated);
                var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

                await _emailService.SendMailResetPassword(user.UserName, user.Email, codeEncoded);
                return Ok(new ApiSuccessResult<string>());
            }


        }
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email does not exist");
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            else
            {
                var codeDecodedBytes = WebEncoders.Base64UrlDecode(model.Code);
                var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);

                var result = await _userManager.ResetPasswordAsync(user, codeDecoded, model.Password);
                if (result.Succeeded)
                {
                    user.PasswordText = model.Password;
                    await _userManager.UpdateAsync(user);

                    return Ok(new ApiSuccessResult<string>());
                }

                AddErrors(result, nameof(model.Email));
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _db.Users.FindAsync(UserId);
            var roles = await _userManager.GetRolesAsync(user);
            var profile = await _db.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == UserId);

            var model = new UserInfoResponse
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Phone = user.PhoneNumber,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                UserName = user.UserName,
                JoinedDate = profile.JoinedDate,
                Roles = roles.ToList(),
                TwoFactor = user.TwoFactorEnabled,
                ApiKey = profile.ApiKey,
            };

            return Ok(new ApiSuccessResult<UserInfoResponse>(model));
        }

        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfile(EditProfileVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            //if (!string.IsNullOrEmpty(model.Phone) && 
            //    await _db.Users.AnyAsync(x => x.PhoneNumber == model.Phone && x.Id != UserId))
            //{
            //    ModelState.AddModelError(nameof(model.Phone), "SĐT đã tồn tại");
            //    return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            //}
			
            var user = await _db.UserProfiles.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.UserId == UserId);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            //user.AppUser.PhoneNumber = model.Phone;
            await _db.SaveChangesAsync();
            return Ok(new ApiSuccessResult<string>());

        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
			
            var user = await _userManager.FindByIdAsync(UserId);

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                user.PasswordText = model.NewPassword;
                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new ApiSuccessResult<string>());
            }

            AddErrors(result, nameof(model.OldPassword));
            return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));

        }
        /// <summary>
        /// Authenticate External(Google,Facebook,...)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("authenticateExternal")]
        public async Task<IActionResult> AuthenticateExternal(UserClaimRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            try
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var user = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower());
                    if (user != null)
                    {
                        var userLogin = await _db.UserLogins.FirstOrDefaultAsync(x => x.UserId == user.Id && x.LoginProvider == model.Type && x.ProviderKey == model.Id);
                        //add provider login
                        if (userLogin == null)
                        {
                            _db.UserLogins.Add(new AppUserLogin
                            {
                                LoginProvider = model.Type,
                                ProviderDisplayName = model.Type,
                                ProviderKey = model.Id,
                                UserId = user.Id,
                            });
                            await _db.SaveChangesAsync();
                        }
                        var jwtToken = await GenerateJwtToken(user);
                        return Ok(new ApiSuccessResult<string>(jwtToken));
                    }
                }

                // check exists user login
                var login = await _db.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == model.Type && x.ProviderKey == model.Id);
                if (login != null)
                {
                    var user = await _db.Users.FindAsync(login.UserId);

                    var jwtToken = await GenerateJwtToken(user);
                    return Ok(new ApiSuccessResult<string>(jwtToken));
                }

                var newUser = new AppUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                newUser.Email = string.IsNullOrEmpty(model.Email) ? null : model.Email;
                newUser.NormalizedEmail = string.IsNullOrEmpty(model.Email) ? null : model.Email.ToUpper();
                newUser.UserName = string.IsNullOrEmpty(model.Email) ? newUser.Id : model.Email;
                newUser.NormalizedUserName = string.IsNullOrEmpty(model.Email) ? newUser.Id : model.Email.ToUpper();

                //insert user
                _db.Users.Add(newUser);

                _db.UserLogins.Add(new AppUserLogin
                {
                    LoginProvider = model.Type,
                    ProviderDisplayName = model.Type,
                    ProviderKey = model.Id,
                    UserId = newUser.Id,
                });

                _db.UserProfiles.Add(new UserProfile
                {
                    CustomId = newUser.Id,
                    FirstName = model.GivenName,
                    LastName = model.Surname,
                    JoinedDate = DateTime.Now,
                    UserId = newUser.Id,
                    ApiKey = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"),
                });

                await _db.SaveChangesAsync();
                return Ok(new ApiSuccessResult<string>(await GenerateJwtToken(newUser)));
            }
            catch (Exception)
            {
                return Ok(new ApiErrorResult<string>($"Login{model.Type} failed! "));
            }

        }

        #region 2fa authentication
        [HttpGet("EnableTwoFactorAuthentication")]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null)
            {
                //2FA Setup
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                var setupInfo = tfa.GenerateSetupCode(SystemConstants.TwoFA.Issuer, user.UserName, TwoFactorKey(user.UserName), false, 3);

                var result = new GoogleAuthenticatorViewModel();
                result.BarcodeUrl = setupInfo.QrCodeSetupImageUrl;
                result.SecretKey = setupInfo.ManualEntryKey;

                await _userManager.SetAuthenticationTokenAsync(user, "GoogleAuthenticator", "Key", result.SecretKey);

                return Ok(new ApiSuccessResult<GoogleAuthenticatorViewModel>(result));
            }

            return Ok(new ApiErrorResult<string>("Not found."));
        }

        [HttpPost("EnableTwoFactorAuthentication")]
        public async Task<IActionResult> EnableTwoFactorAuthentication(string inputCode)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null && user.TwoFactorEnabled == false)
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                bool isValid = tfa.ValidateTwoFactorPIN(TwoFactorKey(user.UserName), inputCode);
                if (isValid)
                {
                    user.TwoFactorEnabled = true;

                    await _userManager.UpdateAsync(user);
                    return Ok(new ApiSuccessResult<string>());
                }

                return Ok(new ApiErrorResult<string>("Invalid code."));
            }

            return Ok(new ApiErrorResult<string>("Not found."));
        }

        [HttpPost("DisableTwoFactorAuthentication")]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            //await UserManager.SetTwoFactorEnabledAsync(UserId, false);
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null)
            {
                user.TwoFactorEnabled = false;

                await _userManager.UpdateAsync(user);
            }
            return Ok(new ApiSuccessResult<string>());
        }

        [AllowAnonymous]
        [HttpPost("VerifyTwoFactorAuthentication")]
        public async Task<IActionResult> VerifyTwoFactorAuthentication(VerifyTFAViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            AppUser user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByIdAsync(UserId);
            }
            else if (!string.IsNullOrEmpty(model.Token))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<Login2FaVerifyObj>(await EncryptionHelper.DecryptAsync(model.Token));
                    if (obj.Exp < DateTime.Now)
                    {
                        return Ok(new ApiErrorResult<string>("Expired login."));
                    }

                    user = await _userManager.FindByIdAsync(obj.UserId);
                }
                catch (Exception)
                {
                    return Ok(new ApiErrorResult<string>("Expired login."));
                }
            }

            if (user != null && user.TwoFactorEnabled)
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                bool isValid = tfa.ValidateTwoFactorPIN(TwoFactorKey(user.UserName), model.Code);
                if (isValid)
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        var jwtToken = await GenerateJwtToken(user);
                        return Ok(new ApiSuccessResult<string>(jwtToken));
                    }
                    return Ok(new ApiSuccessResult<string>());
                }

                return Ok(new ApiErrorResult<string>("Invalid code."));
            }

            return Ok(new ApiErrorResult<string>("Not found."));
        }

        private static string TwoFactorKey(string username)
        {
            return $"{username}_{SystemConstants.TwoFA.PrivateKey2Fa}";
        }
        #endregion


        [HttpPost("ResetApiKey")]
        public async Task<IActionResult> ResetApiKey()
        {
            var user = await _db.UserProfiles.FindAsync(UserId);
            string newAPI = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");
            user.ApiKey = newAPI;
            await _db.SaveChangesAsync();
            return Ok(new ApiSuccessResult<string>());
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

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();


            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));

                //var role = await _roleManager.FindByNameAsync(item);
                //if (role != null)
                //{
                //    claims.AddRange(await _roleManager.GetClaimsAsync(role));
                //}
                var role = await _roleManager.FindByNameAsync(item);
                var claimsRole = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claimsRole)
                {
                    if (!claims.Any(x => x.Type == ApplicationClaimTypes.Permission && x.Value == claim.Value))
                    {
                        claims.Add(claim);
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
             _configuration["Tokens:Issuer"],
             claims,
             expires: DateTime.UtcNow.AddDays(15),
             signingCredentials: creds);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
        #endregion
    }
}
