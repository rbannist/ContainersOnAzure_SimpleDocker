using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend_App.Models;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Frontend_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly Config _config;
        public HomeController(IOptions<Config> config)
        {
            _config = config.Value;
        }
        public async Task<IActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();
            string message = await httpClient.GetAsync(_config.backendUri).Result.Content.ReadAsStringAsync();

            ViewData["Message"] = message;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
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
