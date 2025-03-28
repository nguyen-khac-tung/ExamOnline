using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Lecture.Controllers
{
    [Area("Lecture")]
    public class ExamController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public ExamController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public IActionResult List()
        {
            return View();
        }
    }
}
