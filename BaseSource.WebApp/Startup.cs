using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using BaseSource.Shared.Constants;
using BaseSource.ApiIntegration.WebApi;
using BaseSource.ApiIntegration.AdminApi;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json.Serialization;
using BaseSource.ApiIntegration.SharedApi.Bank;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authorization;
using BaseSource.PermissionBased.Helpers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace BaseSource.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddHttpClient();
            services.AddHttpClient(SystemConstants.AppSettings.BackendApiClient, (sp, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(Configuration.GetValue<string>("BackendApiBaseAddress"));

                // Find the HttpContextAccessor service
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                // Get the bearer token from the request context
                var bearerToken = httpContextAccessor.HttpContext.Request.Cookies[SystemConstants.AppSettings.Token];

                // Add authorization if found
                if (bearerToken != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                // language
                httpClient.DefaultRequestHeaders.Add("Accept-Language",
                    httpContextAccessor.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName]?.Substring(2, 2)?.ToString()
                    ?? SystemConstants.LanguageDefault);
            });

            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(x =>
                {
                    // serialize enums as strings in api responses (e.g. Role)
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Account/Login";
               options.AccessDeniedPath = "/Account/Login/";
           }).AddGoogle(options =>
           {
               // google Authentication
               options.ClientId = Configuration["Authentication:Google:ClientId"];
               options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
               options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "surname");
               options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
           })
           .AddFacebook(options =>
           {
               options.AppId = Configuration["Authentication:Facebook:AppId"];
               options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
           });


            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/Login";
                //options.Cookie.Name = "YourAppCookieName";
                //options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(15);
                options.LoginPath = "/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                //options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                //options.SlidingExpiration = true;
            });

            //config multiple languges
            services.Configure<RequestLocalizationOptions>(
                opt =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo(SystemConstants.LangId.vi),
                        new CultureInfo(SystemConstants.LangId.en)
                    };
                    foreach (var ci in supportedCultures)
                    {
                        ci.NumberFormat.NumberDecimalSeparator = ".";
                        ci.NumberFormat.NumberGroupSeparator = ",";
                        ci.NumberFormat.CurrencyDecimalSeparator = ".";
                        ci.NumberFormat.CurrencyGroupSeparator = ",";
                        ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                        ci.DateTimeFormat.DateSeparator = "/";
                    }

                    opt.DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault());
                    opt.SupportedCultures = supportedCultures;
                    opt.SupportedUICultures = supportedCultures;
                    opt.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()
                    };

                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<ISettingApiClient, SettingApiClient>();
            services.AddTransient<IDashboardApiClient, DashboardApiClient>();
            services.AddTransient<IBankApiClient, BankApiClient>();

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();
            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
