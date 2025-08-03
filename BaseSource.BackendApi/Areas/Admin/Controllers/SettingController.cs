using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Admin;
using BaseSource.Shared.Enums;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace BaseSource.BackendApi.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class SettingController : BaseAdminApiController
    {
        private readonly BaseSourceDbContext _db;

        public SettingController(BaseSourceDbContext context)
        {
            _db = context;
        }


        [HttpGet("GetAlls")]
        public async Task<IActionResult> GetAlls()
        {
            ConfigViewModel _configViewModel = new ConfigViewModel();
            Type type = typeof(ConfigViewModel);
            var list = await _db.Settings.ToListAsync();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    string key = item.Id.Trim();
                    object value = item.Value;
                    var pInfo = type.GetProperty(key);
                    if (pInfo != null)
                    {
                        var pType = pInfo.PropertyType;
                        var targetType = pType.IsGenericType && pType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(pType) : pType;
                        if (value != null)
                        {
                            if (targetType.IsEnum)
                                value = Convert.ToByte(value);
                            else
                                value = Convert.ChangeType(value, targetType);
                            pInfo.SetValue(_configViewModel, value, null);
                        }
                    }
                }
            }

            return Ok(new ApiSuccessResult<ConfigViewModel>(_configViewModel));
        }

        [HttpPost("UpdateConfig")]
        public async Task<IActionResult> UpdateConfig(ConfigViewModel _configViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            try
            {
                var list = await _db.Settings.ToListAsync();
                Type type = _configViewModel.GetType();
                foreach (var item in type.GetProperties())
                {
                    var setting = list.FirstOrDefault(x => x.Id == item.Name);
                    if (setting != null)
                    {
                        object objValue = item.GetValue(_configViewModel, null);
                        if (objValue != null)
                        {
                            var value = item.GetValue(_configViewModel, null).ToString();
                            setting.Value = value;
                        }
                    }
                    else
                    {
                        object objValue = item.GetValue(_configViewModel, null);
                        if (objValue != null)
                        {
                            var value = item.GetValue(_configViewModel, null).ToString();

                            setting = new Setting()
                            {
                                Id = item.Name,
                                Value = value
                            };
                            _db.Settings.Add(setting);
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(new ApiErrorResult<string>(ex.Message));
            }

            return Ok(new ApiSuccessResult<string>());
        }
    }
}
