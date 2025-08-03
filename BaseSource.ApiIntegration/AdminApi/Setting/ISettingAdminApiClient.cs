using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.AdminApi
{
    public interface ISettingAdminApiClient
    {
        Task<ApiResult<ConfigViewModel>> GetAlls();

        Task<ApiResult<string>> UpdateConfig(ConfigViewModel model);
    }
}
