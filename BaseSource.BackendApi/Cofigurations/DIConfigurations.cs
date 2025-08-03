using BaseSource.BackendApi.Services;
using BaseSource.BackendApi.Services.Email;
using BaseSource.BackendApi.Services.Upload;
using Microsoft.Extensions.DependencyInjection;

namespace BaseSource.BackendApi.Cofigurations
{
    public static class DIConfigurations
    {
        public static IServiceCollection DIConfiguration(this IServiceCollection services)
        {
            services.AddTransient<ISendEmailService, SendEmailService>();
            services.AddTransient<IUploadService, UploadService>();

            services.AddTransient<IVietQrService, VietQrService>();

            return services;
        }
    }
}
