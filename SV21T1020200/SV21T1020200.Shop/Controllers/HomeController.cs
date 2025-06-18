using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;
using SV21T1020200.Shop.Models;
using System.Diagnostics;

namespace SV21T1020200.Shop.Controllers
{
    public class HomeController : Controller
    {
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var condition = SV21T1020200.BussinessLayers.ProductDataService.GetBestSeller();
            return View(condition);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
