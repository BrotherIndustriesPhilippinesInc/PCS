using Microsoft.AspNetCore.Mvc;

namespace PartsControlSystem.Controllers
{
    public class PartsDownloadController : Controller
    {
        public IActionResult DownloadPartsData()
        {
            return View();
        }
    }
}
