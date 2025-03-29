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

        [HttpGet]
        public IActionResult TakeExam(string id)
        {
            int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var exam = _studentExamRepository.GetExamWithQuestions(id);

            if (exam.StartDate > DateTime.Now || exam.EndDate < DateTime.Now)
            {
                TempData["ErrorMessage"] = "The exam is not currently available.";
                return RedirectToAction("ListCurrentExam");
            }

            var userExam = new Models.UserExam
            {
                UserId = studentId,
                ExamId = id,
                StartTime = DateTime.Now,
                TimeTaken = 0,
                StatusId = 2, // In Progress
                CorrectAnswer = 0
            };

            _studentExamRepository.CreateUserExam(userExam);

            var model = new TakeExamViewModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                Duration = exam.Duration,
                TotalQuestions = exam.TotalQuestion ?? 0,
                EndTime = exam.EndDate,
                Questions = exam.ExamQuestions.Select(eq => new ExamQuestionViewModel
                {
                    QuestionId = eq.QuestionId,
                    Content = eq.Question.Content,
                    Options = eq.Question.Options.Select(o => new ExamOptionViewModel
                    {
                        OptionId = o.OptionId,
                        Content = o.Content
                    }).ToList()
                }).ToList()
            };

            HttpContext.Session.SetString("ExamStartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitExam(SubmitExamViewModel model)
        {
            int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var exam = _studentExamRepository.GetExamWithQuestions(model.ExamId);

            var userExam = _studentExamRepository.GetUserExam(studentId, model.ExamId);

            string startTimeStr = HttpContext.Session.GetString("ExamStartTime");
            DateTime endTime = DateTime.Now;
            userExam.EndTime = endTime;
            if (DateTime.TryParse(startTimeStr, out DateTime startTime))
            {
                TimeSpan timeTaken = endTime - startTime;
                userExam.TimeTaken = (int)timeTaken.TotalMinutes;
            }

            // Lưu các câu trả lời của người dùng
            int correctAnswers = 0;
            if (model.Answers != null)
            {
                foreach (var answer in model.Answers)
                {
                    // Lưu câu trả lời
                    var userAnswer = new Models.UserAnswer
                    {
                        UserId = studentId,
                        ExamId = model.ExamId,
                        QuestionId = answer.QuestionId,
                        OptionId = answer.SelectedOptionId
                    };
                    _studentExamRepository.SaveUserAnswer(userAnswer);

                    // Kiểm tra đáp án đúng
                    var question = exam.ExamQuestions.FirstOrDefault(eq => eq.QuestionId == answer.QuestionId)?.Question;
                    if (question != null)
                    {
                        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect == true);
                        if (correctOption != null && correctOption.OptionId == answer.SelectedOptionId)
                        {
                            correctAnswers++;
                        }
                    }
                }
            }

            // Cập nhật UserExam
            userExam.CorrectAnswer = correctAnswers;
            userExam.StatusId = 1; // Completed
            _studentExamRepository.UpdateUserExam(userExam);

            // Chuyển tới trang kết quả
            return RedirectToAction("OverviewResult", new { id = model.ExamId });
        }

        [HttpGet]
        public IActionResult OverviewResult(string id)
        {
            int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var result = _studentExamRepository.GetExamResult(studentId, id);
            if (result == null)
            {
                TempData["ErrorMessage"] = "Exam result not found.";
                return RedirectToAction("ListCurrentExam");
            }

            return View(result);
        }
    }
}
