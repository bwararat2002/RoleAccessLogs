using System.Diagnostics;
using CCP.RoleAccessScanner.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace TEST.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string PrivacyRemark = "หน้าความเป็นส่วนตัวของคุณ";
        private const string HomeIndexRemark = "หน้าแรกของคุณ";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [RemarkPage(HomeIndexRemark)]
        public IActionResult Index()
        {
            ViewBag.HomeIndexRemark = HomeIndexRemark;
            return View();
        }

        [Authorize("Admin")]
        [RemarkPage(PrivacyRemark)]
        public IActionResult Privacy()
        {
            ViewBag.PrivacyRemark = PrivacyRemark;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SubmitButton()
        {
            return View();
        }
    }
}
