using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
