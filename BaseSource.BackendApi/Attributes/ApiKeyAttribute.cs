//using BaseSource.Data.EF;
//using BaseSource.Shared.Constants;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BaseSource.BackendApi.Attributes
//{
//    public class SkipApiKeyAttribute : Attribute
//    {
//    }

//    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
//    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
//    {
//        public string IsBot { get; set; }

//        private const string APIKEYNAME = "ApiKey";
//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            if (HasSkipApiKey(context))
//            {
//                await next();
//            }

//            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
//            {
//                context.Result = new ContentResult()
//                {
//                    StatusCode = 401,
//                    Content = "Api Key was not provided"
//                };
//                return;
//            }

//            var _configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

//            var optionsBuilder = new DbContextOptionsBuilder<BaseSourceDbContext>();
//            optionsBuilder.UseSqlServer(_configuration.GetConnectionString(SystemConstants.MainConnectionString));

//            using (BaseSourceDbContext _db = new BaseSourceDbContext(optionsBuilder.Options))
//            {
//                var apiKey = extractedApiKey.ToString();
//                var user = await _db.UserProfiles.Include(x => x.AppUser)
//                    .FirstOrDefaultAsync(x => x.ApiKey == apiKey);

//                if (user == null || user.AppUser.LockoutEnd != null)
//                {
//                    context.Result = new ContentResult()
//                    {
//                        StatusCode = 401,
//                        Content = "Unauthorized client"
//                    };
//                    return;
//                }

//                if (!string.IsNullOrEmpty(IsBot) &&
//                    IsBot == "true")
//                {
//                    if (!user.IsBotAccount)
//                    {
//                        context.Result = new ContentResult()
//                        {
//                            StatusCode = 401,
//                            Content = "Unauthorized client"
//                        };
//                        return;
//                    }
//                }

//                if (!context.HttpContext.Items.Any(x => x.Key.ToString() == "UserId"))
//                {
//                    context.HttpContext.Items.Add("UserId", user.UserId);
//                }
//            }

//            await next();
//        }

//        private static bool HasSkipApiKey(ActionExecutingContext context)
//        {
//            var filters = context.Filters;
//            for (var i = 0; i < filters.Count; i++)
//            {
//                if (filters[i] is SkipApiKeyAttribute)
//                {
//                    return true;
//                }
//            }

//            // When doing endpoint routing, MVC does not add AllowAnonymousFilters for AllowAnonymousAttributes that
//            // were discovered on controllers and actions. To maintain compat with 2.x,
//            // we'll check for the presence of IAllowAnonymous in endpoint metadata.
//            var endpoint = context.HttpContext.GetEndpoint();
//            if (endpoint?.Metadata?.GetMetadata<SkipApiKeyAttribute>() != null)
//            {
//                return true;
//            }

//            return false;
//        }
//    }
//}
