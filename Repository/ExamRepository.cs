using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Repository
{
    public interface IExamRepository
    {
        List<ExamListViewModel> GetExamsByLectureId(int lectureId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords);
        void DeleteExam(string examId);
        //Exam? GetExamById(string examId);
    }

    public class ExamRepository: IExamRepository
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

        public void DeleteExam(string examId)
        {
            var exam = _context.Exams.Find(examId);
            exam.IsDelete = true;
            _context.SaveChanges();
        }
    }
}
