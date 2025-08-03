using BaseSource.ApiIntegration.WebApi;
using BaseSource.Shared.Constants;
using BaseSource.Utilities.Helper;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Shared.Account;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BaseSource.Shared.Constants.SystemConstants;

namespace BaseSource.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AccountController(IUserApiClient userApiClient,
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ClearAuthorizedCookies();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userApiClient.Register(model);

            if (result.IsSuccessed)
            {
                //var message = new MessageResult()
                //{
                //    Title = "Đăng ký thành công",
                //    Content = "Một email đã gửi đến cho bạn. Vui lòng xác thực email bằng đường link đã gủi đến email."
                //};
                //return View("MessageViewResult", message);
                return RedirectToAction("Login");
            }

            ModelState.AddListErrors(result.ValidationErrors);
            return View(model);

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestVm model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userApiClient.Authenticate(model);
            if (!result.IsSuccessed)
            {
                ModelState.AddListErrors(result.ValidationErrors);
                return View(model);
            }

            if (result.ResultObj.TwoFactorEnabled)
            {
                HttpContext.Response.Cookies.Append("VerifiedUser", result.ResultObj.Token, new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.UtcNow.AddMinutes(15) });

                return RedirectToAction("VerifyTFA", new { ReturnUrl = returnUrl });
            }

            //signin cookie
            await SignInCookie(result.ResultObj.Token);
            return RedirectToLocal(returnUrl);

        }

        public async Task<IActionResult> Logout()
        {
            ClearAuthorizedCookies();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var ob = new ConfirmEmailVm()
            {
                UserId = userId,
                Code = code
            };
            var result = await _userApiClient.ConfirmEmail(ob);

            var message = new MessageResult();
            if (result.IsSuccessed)
            {
                message.Title = "Xác thực thành công";
                message.Content = "Xác thực thành công. Vui lòng click <a href=\"/Account/Login\"> vào đây</a> để tiếp tục.";
            }
            else
            {
                message.Title = "Xác thực thất bại";
                message.Content = "Xác thực thất bại. Vui lòng xác thực lại.";
            }

            return View("MessageViewResult", message);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);

            }
            var result = await _userApiClient.ForgotPassword(model);
            if (result.IsSuccessed)
            {
                var message = new MessageResult()
                {
                    Title = "Quên mật khẩu",
                    Content = "Vui lòng click đường link trong email để đổi mật khẩu."
                };

                return View("MessageViewResult", message);
            }
            ModelState.AddListErrors(result.ValidationErrors);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string email)
        {
            var model = new ResetPasswordVm
            {
                Code = code,
                Email = email
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userApiClient.ResetPassword(model);
            if (result.IsSuccessed)
            {
                var message = new MessageResult()
                {
                    Title = "Đổi mật khẩu",
                    Content = "Đổi mật khẩu thành công. Vui lòng click <a href=\"/Account/Login\"> vào đây</a> để tiếp tục."
                };

                return View("MessageViewResult", message);
            }
            ModelState.AddListErrors(result.ValidationErrors);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("google-login")]
        public IActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl = returnUrl })
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// response google authencation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl)
        {
            try
            {
                //get clamis User Google
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var claims = result.Principal.Identities
                .Select(x => new UserClaimRequest
                {
                    Id = x.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = x.FindFirst(ClaimTypes.Email)?.Value,
                    GivenName = x.FindFirst(ClaimTypes.GivenName).Value,
                    Surname = x.FindFirst(ClaimTypes.Surname)?.Value,
                    Type = x.Claims.FirstOrDefault().Issuer
                }).FirstOrDefault();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                //google Authencation API
                var resultApi = await _userApiClient.AuthenticateExternalAsync(claims);
                if (!resultApi.IsSuccessed)
                {
                    return View("MessageViewResult", new MessageResult { Title = "Đăng nhập không thành công!", Content = "Đăng nhập Google không thành công!" });
                }
                //signin cookie
                await SignInCookie(resultApi.ResultObj);
                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                return View("MessageViewResult", new MessageResult { Title = "Đăng nhập không thành công!", Content = "Đăng nhập Google không thành công!" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("facebook-login")]
        public IActionResult FacebookLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse", new { returnUrl = returnUrl }) };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// response facebook authencation
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [Route("facebook-response")]
        public async Task<IActionResult> FacebookResponse(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //get claims user facebook
            var claims = result.Principal.Identities
                .Select(x => new UserClaimRequest
                {
                    Id = x.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = x.FindFirst(ClaimTypes.Email)?.Value,
                    GivenName = x.FindFirst(ClaimTypes.GivenName).Value,
                    Surname = x.FindFirst(ClaimTypes.Surname)?.Value,
                    Type = x.Claims.FirstOrDefault().Issuer
                }).FirstOrDefault();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //facebook Authencation API
            var resultApi = await _userApiClient.AuthenticateExternalAsync(claims);
            if (!resultApi.IsSuccessed)
            {
                return View("MessageViewResult", new MessageResult { Title = "Đăng nhập không thành công!", Content = "Đăng nhập Facebook không thành công!" });
            }
            //signin cookie
            await SignInCookie(resultApi.ResultObj);
            return RedirectToLocal(returnUrl);
        }

        #region 2fa
        //
        // GET: /Account/VerifyTFA
        [AllowAnonymous]
        public IActionResult VerifyTFA(string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (Request.Cookies["VerifiedUser"] == null)
            {
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            return View(new VerifyTFAViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> VerifyTFA(VerifyTFAViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require that the user has already logged in via username/password or external login
            if (Request.Cookies["VerifiedUser"] == null)
            {
                return RedirectToAction("Login", new { ReturnUrl = model.ReturnUrl });
            }

            model.Token = Request.Cookies["VerifiedUser"];
            var result = await _userApiClient.VerifyTwoFactorAuthentication(model);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("Code", result.Message);
                return View(model);
            }

            //signin cookie
            await SignInCookie(result.ResultObj);

            Response.Cookies.Delete("VerifiedUser");
            return RedirectToLocal(model.ReturnUrl);
        }
        #endregion

        #region helper
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void ClearAuthorizedCookies()
        {
            Response.Cookies.Append(AppSettings.Token, "", new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }
        private async Task SignInCookie(string token)
        {
            var userPrincipal = this.ValidateToken(token);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15),
                IsPersistent = false
            };
            HttpContext.Response.Cookies.Append(SystemConstants.AppSettings.Token, token, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(15) });
            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal, authProperties);
        }
        #endregion
    }
}
