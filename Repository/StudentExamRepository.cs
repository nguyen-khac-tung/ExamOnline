using ExamationOnline.Areas.Student.ViewModels;
using ExamationOnline.Models;

namespace ExamationOnline.Repository
{
    public interface IStudentExamRepository
    {
        List<StudentExamListViewModel> GetCurrentExams(int studentId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords);
        bool HasStudentTakenExam(int studentId, string examId);
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
    }
}
