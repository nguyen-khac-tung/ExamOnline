using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Repository
{
    public interface IExamRepository
    {
        List<ExamListViewModel> GetExamsByLectureId(int lectureId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords);
        Exam? GetExamDetailWithQuestions(string examId);
        void DeleteExam(string examId);
        string CreateExam(ExamViewModel model);
        Exam GetExamById(string examId);
        List<string> GetSelectedQuestionIds(string examId);
        void UpdateExamQuestions(string examId, List<string> questionIds);
        bool IsExamTakenByAnyStudent(string examId);
        ExamViewModel GetExamViewModelForEdit(string examId);
        int? UpdateExam(ExamViewModel model);
    }

    public class ExamRepository : IExamRepository
    {
        private readonly ExamOnlineContext _context;

        public ExamRepository(ExamOnlineContext context)
        {
            _context = context;
        }

        public List<ExamListViewModel> GetExamsByLectureId(int lectureId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords)
        {
            var query = _context.Exams
                .Include(e => e.Status)
                .Where(e => e.LectureId == lectureId && e.IsDelete != true)
                .Select(e => new ExamListViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Duration = e.Duration,
                    TotalQuestion = e.TotalQuestion,
                    CreateDate = e.CreateDate,
                    StatusName = e.Status.StatusName,
                    StatusId = e.StatusId,
                    CanDelete = e.StatusId == 3
                });

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim().ToLower();
                query = query.Where(e => e.ExamName.ToLower().Contains(searchQuery));
            }

            totalRecords = query.Count();

            query = OrderListExam(query, sortColumn, isAsc);

            var pagedExams = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return pagedExams;
        }

        private IQueryable<ExamListViewModel> OrderListExam(IQueryable<ExamListViewModel> query,
            string? sortColumn, bool isAsc)
        {
            query = query.OrderByDescending(e => e.CreateDate);

            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "examname":
                        query = isAsc ? query.OrderBy(e => e.ExamName) : query.OrderByDescending(e => e.ExamName);
                        break;
                    case "startdate":
                        query = isAsc ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate);
                        break;
                    case "enddate":
                        query = isAsc ? query.OrderBy(e => e.EndDate) : query.OrderByDescending(e => e.EndDate);
                        break;
                    case "duration":
                        query = isAsc ? query.OrderBy(e => e.Duration) : query.OrderByDescending(e => e.Duration);
                        break;
                    case "totalquestion":
                        query = isAsc ? query.OrderBy(e => e.TotalQuestion) : query.OrderByDescending(e => e.TotalQuestion);
                        break;
                    case "createdate":
                        query = isAsc ? query.OrderBy(e => e.CreateDate) : query.OrderByDescending(e => e.CreateDate);
                        break;
                    case "statusname":
                        query = isAsc ? query.OrderBy(e => e.StatusName) : query.OrderByDescending(e => e.StatusName);
                        break;
                }
            }

            return query;
        }

        public Exam? GetExamDetailWithQuestions(string examId)
        {
            return _context.Exams
                .Include(e => e.Status)
                .Include(e => e.Subject)
                .Include(e => e.Class)
                .Include(e => e.Lecture)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.Options)
                .FirstOrDefault(e => e.ExamId == examId);
        }

        public void DeleteExam(string examId)
        {
            var exam = _context.Exams.Find(examId);
            exam.IsDelete = true;
            _context.SaveChanges();
        }

        public string CreateExam(ExamViewModel model)
        {
            string examId = Guid.NewGuid().ToString();

            if (model.StartDate > DateTime.Now) model.StatusId = 3;
            else model.StatusId = 2;

            var exam = new Exam
            {
                ExamId = examId,
                ExamName = model.ExamName,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Duration = model.Duration,
                ClassId = model.ClassId,
                SubjectId = model.SubjectId,
                IsDisplayAnswer = model.IsDisplayAnswer,
                StatusId = model.StatusId,
                LectureId = model.LectureId,
                CreateDate = DateTime.Now,
                IsDelete = false,
                TotalQuestion = 0
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();
            return examId;
        }

        public Exam? GetExamById(string examId)
        {
            return _context.Exams.FirstOrDefault(e => e.ExamId == examId);
        }

        public List<string> GetSelectedQuestionIds(string examId)
        {
            return _context.ExamQuestions
                .Where(eq => eq.ExamId == examId)
                .Select(eq => eq.QuestionId)
                .ToList();
        }

        public void UpdateExamQuestions(string examId, List<string> questionIds)
        {
            var oldListQuestion = _context.ExamQuestions.Where(eq => eq.ExamId == examId);
            _context.ExamQuestions.RemoveRange(oldListQuestion);

            foreach (var questionId in questionIds)
            {
                var eq = new ExamQuestion()
                {
                    ExamId = examId,
                    QuestionId = questionId
                };

                _context.ExamQuestions.Add(eq);
            }

            // Update total questions count
            var exam = _context.Exams.Find(examId);
            exam.TotalQuestion = questionIds.Count;

            _context.SaveChanges();
        }

        public bool IsExamTakenByAnyStudent(string examId)
        {
            return _context.UserExams.Any(ue => ue.ExamId == examId);
        }

        public ExamViewModel GetExamViewModelForEdit(string examId)
        {
            var exam = _context.Exams.Find(examId);
            bool isExamTaken = IsExamTakenByAnyStudent(examId);

            var model = new ExamViewModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                Duration = exam.Duration,
                ClassId = exam.ClassId,
                SubjectId = exam.SubjectId,
                IsDisplayAnswer = exam.IsDisplayAnswer ?? false,
                StatusId = exam.StatusId,
                IsExamTaken = isExamTaken,
            };

            return model;
        }

        public int? UpdateExam(ExamViewModel model)
        {
            var oldExam = _context.Exams.Find(model.ExamId);

            // These fields can always be updated
            oldExam.ExamName = model.ExamName;
            oldExam.EndDate = model.EndDate;
            oldExam.Duration = model.Duration;
            oldExam.IsDisplayAnswer = model.IsDisplayAnswer;

            // If exam is not taken or completed, we can update all fields
            if (model.IsExamTaken == false)
            {
                oldExam.StartDate = model.StartDate;
                oldExam.ClassId = model.ClassId;
                oldExam.SubjectId = model.SubjectId;
            }

            // Update status based on start date
            if (model.StartDate > DateTime.Now)
                oldExam.StatusId = 3; // Upcoming
            else if (model.EndDate < DateTime.Now)
                oldExam.StatusId = 1; // Ended
            else 
                oldExam.StatusId = 2; // In progress

            _context.SaveChanges();
            return oldExam.StatusId;
        }
    }
}
