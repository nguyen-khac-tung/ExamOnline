using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Areas.Lecture.Controllers
{
    [Area("Lecture")]
    public class ExamController : Controller
    {
        private readonly IExamRepository _examRepository;

        public ExamController(IExamRepository examRepository)
        {
            _examRepository = examRepository;
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
        public IActionResult Delete(string id)
        {
            _examRepository.DeleteExam(id);
            TempData["SuccessMessage"] = "Question deleted successfully!";
            return RedirectToAction("List");
        }
    }
}
