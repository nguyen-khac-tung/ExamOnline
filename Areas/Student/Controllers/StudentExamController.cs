using ExamationOnline.Areas.Student.ViewModels;
using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentExamController : Controller
    {
        private readonly IStudentExamRepository _studentExamRepository;

        public StudentExamController(IStudentExamRepository studentExamRepository)
        {
            _studentExamRepository = studentExamRepository;
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
    }
}
