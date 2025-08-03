using BaseSource.AdminApp.Controllers;
using BaseSource.PermissionBased.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.AdminApp.Areas.Admin.Controllers
{
    [Authorize(Policy = Permissions.AdminPage.View)]
    [Area("Admin")]
    public class BaseAdminController : BaseController
    {
    }
}
