using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi
{
    public interface ISettingApiClient
    {
        Task<ApiResult<SettingPublicVm>> GetAll();
    }
}
