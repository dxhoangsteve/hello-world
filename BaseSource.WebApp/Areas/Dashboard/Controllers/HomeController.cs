using BaseSource.ApiIntegration.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseSource.WebApp.Areas.Dashboard.Controllers
{
    public class HomeController : BaseDashboardController
    {
        private readonly IDashboardApiClient _apiClient;
        public HomeController(IDashboardApiClient apiClient)
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
