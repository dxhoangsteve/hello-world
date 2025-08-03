using BaseSource.ApiIntegration.AdminApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseSource.AdminApp.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly IDashboardAdminApiClient _apiClient;
        public HomeController(IDashboardAdminApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var rsStats = await _apiClient.GetStats();
            return View(rsStats.ResultObj);
        }
    }
}
