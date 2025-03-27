using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExamationOnline.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
