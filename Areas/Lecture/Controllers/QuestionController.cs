using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Lecture.Controllers
{
    [Area("Lecture")]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult GetPartialViewListing(string search = "", int page = 1, int pageSize = 10, string sortBy = "", string sortDir = "desc")
        {
            int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (lectureId == 0)
            {
                return PartialView("_QuestionListPartial", new QuestionListingViewModel
                {
                    Questions = new List<QuestionListViewModel>(),
                    TotalRecords = 0,
                    CurrentPage = 1,
                    PageSize = pageSize
                });
            }

            bool isAscending = sortDir.ToLower() == "asc";

            var questions = _questionRepository.GetQuestionsByLectureId(
                lectureId, search, sortBy, isAscending, pageSize, page, out int totalRecords);

            var model = new QuestionListingViewModel
            {
                Questions = questions,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                SearchQuery = search,
                SortColumn = sortBy,
                IsAscending = isAscending
            };

            return PartialView("_QuestionListPartial", model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            var question = _questionRepository.GetQuestionById(id);
            if (question == null)
            {
                TempData["ErrorMessage"] = "Question not found.";
                return RedirectToAction("List");
            }
            return View(question);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new QuestionViewModel();
            model.Options.Add(new OptionViewModel());
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Options.Any())
            {
                if (model.Options.Count < 2)
                {
                    ModelState.AddModelError("", "You must provide at least 2 options if you want to add options.");
                    return View(model);
                }

                if (!model.Options.Any(o => o.IsCorrect == true))
                {
                    ModelState.AddModelError("", "You must select at least one correct answer.");
                    return View(model);
                }
            }

            int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;
            model.LectureID = lectureId;
            string questionId = _questionRepository.CreateQuestion(model);
            return RedirectToAction("Detail", new { id = questionId });
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var model = _questionRepository.GetQuestionVMById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Options.Any())
            {
                if (model.Options.Count < 2)
                {
                    ModelState.AddModelError("", "You must provide at least 2 options if you want to add options.");
                    return View(model);
                }

                if (!model.Options.Any(o => o.IsCorrect == true))
                {
                    ModelState.AddModelError("", "You must select at least one correct answer.");
                    return View(model);
                }
            }

            string questionId = _questionRepository.CreateQuestion(model);
            return RedirectToAction("Detail", new { id = questionId });
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (_questionRepository.IsQuestionInExam(id))
            {
                TempData["ErrorMessage"] = "Can't delete this question because it is used in exam(s).";
                return RedirectToAction("List");
            }

            _questionRepository.DeleteQuestion(id);
            TempData["SuccessMessage"] = "Question deleted successfully!";
            return RedirectToAction("List");
        }

    }
}
