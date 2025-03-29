using ExamationOnline.Areas.Student.ViewModels;
using ExamationOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Repository
{
    public interface IStudentExamRepository
    {
        List<StudentExamListViewModel> GetCurrentExams(int studentId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords);
        bool HasStudentTakenExam(int studentId, string examId);
        Exam? GetExamById(string examId);
        Exam? GetExamWithQuestions(string examId);
        void CreateUserExam(UserExam userExam);
        UserExam? GetUserExam(int userId, string examId);
        void SaveUserAnswer(UserAnswer userAnswer);
        void UpdateUserExam(UserExam userExam);
        ExamResultViewModel GetExamResult(int userId, string examId);
    }

    public class StudentExamRepository : IStudentExamRepository
    {
        private readonly ExamOnlineContext _context;

        public StudentExamRepository(ExamOnlineContext context)
        {
            _context = context;
        }

        public List<StudentExamListViewModel> GetCurrentExams(int studentId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords)
        {
            // Get classes for the student
            var studentClasses = _context.ClassUsers
                .Where(cu => cu.UserId == studentId)
                .Select(cu => cu.ClassId)
                .ToList();

            // Get subjects for the student
            var studentSubjects = _context.SubjectUsers
                .Where(su => su.UserId == studentId)
                .Select(su => su.SubjectId)
                .ToList();

            var examHasTaken = _context.UserExams.Where(ue => ue.UserId == studentId)
                                                  .Select(ue => ue.ExamId)
                                                  .ToList();

            // Current time
            var now = DateTime.Now;

            var query = _context.Exams
                .Where(e => e.IsDelete != true
                    && studentClasses.Contains(e.ClassId ?? 0)
                    && studentSubjects.Contains(e.SubjectId ?? 0)
                    && e.StartDate <= now
                    && e.EndDate >= now
                    && !examHasTaken.Contains(e.ExamId))
                .Select(e => new StudentExamListViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Duration = e.Duration,
                    TotalQuestion = e.TotalQuestion,
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

        private IQueryable<StudentExamListViewModel> OrderListExam(IQueryable<StudentExamListViewModel> query,
            string? sortColumn, bool isAsc)
        {
            query = query.OrderByDescending(e => e.StartDate);

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
                }
            }

            return query;
        }

        public bool HasStudentTakenExam(int studentId, string examId)
        {
            return _context.UserExams.Any(ue => ue.UserId == studentId && ue.ExamId == examId);
        }

        public Exam? GetExamById(string examId)
        {
            return _context.Exams
                .Include(e => e.Class)
                .Include(e => e.Subject)
                .FirstOrDefault(e => e.ExamId == examId);
        }

        public Exam? GetExamWithQuestions(string examId)
        {
            return _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.Options)
                .FirstOrDefault(e => e.ExamId == examId);
        }

        public void CreateUserExam(UserExam userExam)
        {
            _context.UserExams.Add(userExam);
            _context.SaveChanges();
        }

        public UserExam? GetUserExam(int userId, string examId)
        {
            return _context.UserExams
                .FirstOrDefault(ue => ue.UserId == userId && ue.ExamId == examId);
        }

        public void SaveUserAnswer(UserAnswer userAnswer)
        {
            _context.UserAnswers.Add(userAnswer);
            _context.SaveChanges();
        }

        public void UpdateUserExam(UserExam userExam)
        {
            _context.UserExams.Update(userExam);
            _context.SaveChanges();
        }

        public ExamResultViewModel GetExamResult(int userId, string examId)
        {
            var exam = _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
                        .ThenInclude(q => q.Options)
                .FirstOrDefault(e => e.ExamId == examId);

            var userExam = _context.UserExams
                .FirstOrDefault(ue => ue.UserId == userId && ue.ExamId == examId);

            var userAnswers = _context.UserAnswers
                .Include(ua => ua.Option)
                .Include(ua => ua.Question)
                .Where(ua => ua.UserId == userId && ua.ExamId == examId)
                .ToList();

            if (exam == null || userExam == null)
                return null;

            var result = new ExamResultViewModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                Duration = exam.Duration,
                TimeTaken = userExam.TimeTaken ?? 0,
                TotalQuestions = exam.TotalQuestion ?? 0,
                CorrectAnswers = userExam.CorrectAnswer ?? 0,
                StartTime = userExam.StartTime ?? DateTime.Now,
                EndTime = userExam.EndTime ?? DateTime.Now,
                DisplayAnswers = exam.IsDisplayAnswer ?? false,
                Answers = new List<AnswerResultViewModel>()
            };

            foreach (var eq in exam.ExamQuestions)
            {
                var question = eq.Question;
                var userAnswer = userAnswers.FirstOrDefault(ua => ua.QuestionId == question.QuestionId);
                var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect == true);

                var answerResult = new AnswerResultViewModel
                {
                    QuestionId = question.QuestionId,
                    QuestionContent = question.Content,
                    SelectedOptionId = userAnswer?.OptionId,
                    SelectedOptionContent = userAnswer?.Option?.Content,
                    CorrectOptionId = correctOption?.OptionId,
                    CorrectOptionContent = correctOption?.Content,
                    IsCorrect = userAnswer?.OptionId == correctOption?.OptionId
                };

                result.Answers.Add(answerResult);
            }

            return result;
        }
    }
}
