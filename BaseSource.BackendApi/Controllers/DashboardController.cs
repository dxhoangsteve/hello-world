using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseSource.Data.EF;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using BaseSource.ViewModels.User;
using BaseSource.Shared.Enums;

namespace BaseSource.BackendApi.Controllers
{
    [AllowAnonymous]
    public class DashboardController : BaseApiController
    {
        private readonly BaseSourceDbContext _db;

        public DashboardController(BaseSourceDbContext context)
        {
            _db = context;
        }

        [HttpGet("GetStats")]
        public async Task<IActionResult> GetStats()
        {
            var rs = new DashboardVm()
            {
            };

            return Ok(new ApiSuccessResult<DashboardVm>(rs));
        }
    }
}
