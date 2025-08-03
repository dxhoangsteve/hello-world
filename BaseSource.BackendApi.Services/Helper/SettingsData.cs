using BaseSource.Data.Entities;
using BaseSource.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Services.Helper
{
    public class SettingsData
    {
        public static ConfigViewModel Get(List<Setting> list)
        {
            ConfigViewModel _configViewModel = new ConfigViewModel();
            Type type = typeof(ConfigViewModel);

            if (list.Any())
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

            return _configViewModel;
        }
    }
}
