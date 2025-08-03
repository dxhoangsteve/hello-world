using BaseSource.WebApp.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.WebApp.Areas.Dashboard.Controllers
{
    [Authorize]
    [Area("Dashboard")]
    public class BaseDashboardController : BaseController
    {
    }
}
