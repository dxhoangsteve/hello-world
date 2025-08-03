using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Admin;
using BaseSource.ViewModels.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BaseSource.BackendApi.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminApiController
    {
        private readonly BaseSourceDbContext _db;
        public DashboardController(BaseSourceDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetStats")]
        public async Task<IActionResult> GetStats()
        {
            var rs = new DashboardAdminVm()
            {
                CountUser = await _db.UserProfiles.CountAsync(),
            };

            return Ok(new ApiSuccessResult<DashboardAdminVm>(rs));
        }
    }
}
