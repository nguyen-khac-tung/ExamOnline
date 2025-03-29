using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Areas.Lecture.Controllers
{
    [Area("Lecture")]
    public class ExamController : Controller
    {
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;

        public ExamController(IExamRepository examRepository, IQuestionRepository questionRepository,
            IClassRepository classRepository, ISubjectRepository subjectRepository)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult GetPartialViewListing(string search = "", int page = 1, int pageSize = 10,
            string sortBy = "", string sortDir = "desc")
        {
            int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (lectureId == 0)
            {
                return PartialView("_ExamListPartial", new ExamListingViewModel
                {
                    Exams = new List<ExamListViewModel>(),
                    TotalRecords = 0,
                    CurrentPage = 1,
                    PageSize = pageSize
                });
            }

            bool isAscending = sortDir.ToLower() == "asc";

            var exams = _examRepository.GetExamsByLectureId(
                lectureId, search, sortBy, isAscending, pageSize, page, out int totalRecords);

            var model = new ExamListingViewModel
            {
                Exams = exams,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                SearchQuery = search,
                SortColumn = sortBy,
                IsAscending = isAscending
            };

            return PartialView("_ExamListPartial", model);
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            var exam = _examRepository.GetExamDetailWithQuestions(id);
            if (exam == null)
            {
                TempData["ErrorMessage"] = "Exam not found.";
                return RedirectToAction("List");
            }

            return View(exam);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ExamViewModel
            {
                StartDate = null,
                EndDate = null,
                ClassList = _classRepository.GetListClass(),
                SubjectList = _subjectRepository.GetListSubject(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ExamViewModel model)
        {
            model.ClassList = _classRepository.GetListClass();
            model.SubjectList = _subjectRepository.GetListSubject();

            if (model.StartDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("StartDate", "Start date cannot be in the past");
            }

            if (model.StartDate.Value.AddMinutes(model.Duration) >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be greater than Start date plus Duration");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;
            model.LectureId = lectureId;
            string examId = _examRepository.CreateExam(model);

            return RedirectToAction("AddQuestion", new { id = examId });
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var model = _examRepository.GetExamViewModelForEdit(id);
            model.ClassList = _classRepository.GetListClass();
            model.SubjectList = _subjectRepository.GetListSubject();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ExamViewModel model)
        {
            model.ClassList = _classRepository.GetListClass();
            model.SubjectList = _subjectRepository.GetListSubject();

            if (model.IsExamTaken == false)
            {
                if (model.StartDate.Value < DateTime.Now)
                {
                    ModelState.AddModelError("StartDate", "Start date cannot be in the past");
                }
            }

            if (model.StartDate.Value.AddMinutes(model.Duration) >= model.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be greater than Start date plus Duration");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.StatusId = _examRepository.UpdateExam(model);
            return View(model);
        }

        [HttpGet]
        public IActionResult AddQuestion(string id)
        {
            var exam = _examRepository.GetExamById(id);
            if (exam == null)
            {
                TempData["ErrorMessage"] = "Exam not found.";
                return RedirectToAction("List");
            }

            int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var model = new AddQuestionViewModel
            {
                ExamId = id,
                ExamName = exam.ExamName,
                AvailableQuestions = _questionRepository.GetAvailableQuestionsForExam(lectureId),
                SelectedQuestionIds = _examRepository.GetSelectedQuestionIds(id)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddQuestion(AddQuestionViewModel model)
        {
            if (model.SelectedQuestionIds == null || !model.SelectedQuestionIds.Any())
            {
                int lectureId = HttpContext.Session.GetInt32("UserId") ?? 0;
                model.AvailableQuestions = _questionRepository.GetAvailableQuestionsForExam(lectureId);
                TempData["ErrorMessage"] = "You must select at least one question.";
                return View(model);
            }

            _examRepository.UpdateExamQuestions(model.ExamId, model.SelectedQuestionIds);
            return RedirectToAction("Detail", new { id = model.ExamId });
        }


        [HttpGet]
        public IActionResult Delete(string id)
        {
            _examRepository.DeleteExam(id);
            TempData["SuccessMessage"] = "Question deleted successfully!";
            return RedirectToAction("List");
        }
    }
}
