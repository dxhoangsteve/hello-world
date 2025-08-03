using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Services.Email
{
    public interface ISendEmailService
    {
        Task SendMailConfirmEmail(string username, string email, string userId, string code);
        Task SendMailResetPassword(string username, string email, string code);
    }
}
