using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using ElmahCore;

namespace BaseSource.BackendApi.Services
{
    class MyExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellation)
        {
            await context.RaiseError(exception);

            // Your response object
            var error = new { message = exception.Message };
            await context.Response.WriteAsJsonAsync(error, cancellation);
            return true;
        }
    }
}
