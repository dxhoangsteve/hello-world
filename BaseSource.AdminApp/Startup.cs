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
using System.Text.Json.Serialization;
using BaseSource.PermissionBased.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using BaseSource.ApiIntegration.SharedApi.Bank;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseSource.ApiIntegration.AdminApi.Article;

namespace BaseSource.AdminApp
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
            });
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(x =>
            {
                // serialize enums as strings in api responses (e.g. Role)
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               options.LoginPath = "/Account/Login";
               options.AccessDeniedPath = "/Account/Login/";
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IUserApiClient, UserApiClient>();

            #region admin
            services.AddTransient<IUserAdminApiClient, UserAdminApiClient>();
            services.AddTransient<ISettingAdminApiClient, SettingAdminApiClient>();
            services.AddTransient<IRoleAdminApiClient, RoleAdminApiClient>();

            services.AddTransient<IBankApiClient, BankApiClient>();
            services.AddTransient<IDashboardAdminApiClient, DashboardAdminApiClient>();
            services.AddTransient<IEditorApiClient, EditorApiClient>();
            #endregion

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            //set multi language
            services.Configure<RequestLocalizationOptions>(options =>
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
                    ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                    ci.DateTimeFormat.DateSeparator = "/";
                }

                options.DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });
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

            app.UseRouting();

            app.UseRequestLocalization();

            app.UseAuthentication();

            app.UseAuthorization();

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
