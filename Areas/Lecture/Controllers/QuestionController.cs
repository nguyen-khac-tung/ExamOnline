using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Lecture.Controllers
{
    public class QuestionController : Controller
    {
        [Area("Lecture")]
        public IActionResult List()
        {
            return View();
        }
    }
}
