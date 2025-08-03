using BaseSource.Data.EF;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Services.SignalR
{
    public interface ICommonClientHub
    {
        Task ReceiveMessage(string message);
        Task UserBalanceChanged(UserBalanceDto model);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommonHub : Hub<ICommonClientHub>
    {
        public CommonHub()
        {
        }

        //private string _userId;
        //public string UserId
        //{
        //    get { return _userId ?? Context.User.FindFirstValue(ClaimTypes.NameIdentifier); }
        //    set { _userId = value; }
        //}
    }

    public class UserBalanceDto
    {
        public string Id { get; set; }
        public double Balance { get; set; }
    }
}
