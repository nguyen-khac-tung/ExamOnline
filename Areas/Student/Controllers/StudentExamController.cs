using ExamationOnline.Areas.Student.ViewModels;
using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentExamController : Controller
    {
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly IExamRepository _examRepository;

        public StudentExamController(IStudentExamRepository studentExamRepository, IExamRepository examRepository)
        {
            _studentExamRepository = studentExamRepository;
            _examRepository = examRepository;
        }

        public IActionResult ListCurrentExam()
        {
            return View();
        }

        public IActionResult GetPartialViewListing(string search = "", int page = 1, int pageSize = 10,
            string sortBy = "", string sortDir = "desc")
        {
            int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (studentId == 0)
            {
                return PartialView("_ExamListPartial", new StudentExamListingViewModel
                {
                    Exams = new List<StudentExamListViewModel>(),
                    TotalRecords = 0,
                    CurrentPage = 1,
                    PageSize = pageSize
                });
            }

            bool isAscending = sortDir.ToLower() == "asc";

            var exams = _studentExamRepository.GetCurrentExams(
                studentId, search, sortBy, isAscending, pageSize, page, out int totalRecords);

            var model = new StudentExamListingViewModel
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
        public IActionResult ConfirmTakeExam(string id)
        {
            var exam = _examRepository.GetExamById(id);
            if (exam == null)
            {
                TempData["ErrorMessage"] = "Exam not found.";
                return RedirectToAction("ListCurrentExam");
            }

            return View(exam);
        }
    }
}
