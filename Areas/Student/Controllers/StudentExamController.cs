using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Student.Controllers
{
    public class StudentExamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
