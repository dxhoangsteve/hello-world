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
using Microsoft.AspNetCore.Authorization;
using BaseSource.BackendApi.Services.Helper;
using BaseSource.ViewModels.User;

namespace BaseSource.BackendApi.Controllers
{
    [AllowAnonymous]
    public class SettingController : BaseApiController
    {
        private readonly BaseSourceDbContext _db;

        public SettingController(BaseSourceDbContext context)
        {
            _db = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var settings = SettingsData.Get(await _db.Settings.ToListAsync());
            var model = new SettingPublicVm()
            {
                ContactEmail = settings.ContactEmail,
                ContactPhone = settings.ContactPhone,
                SupportUrl = settings.SupportUrl
            };
            return Ok(new ApiSuccessResult<SettingPublicVm>(model));
        }
    }
}
