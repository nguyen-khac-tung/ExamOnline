using ExamationOnline.Areas.Lecture.ViewModels;
using ExamationOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamationOnline.Repository
{
    public interface IQuestionRepository
    {
        List<QuestionListViewModel> GetQuestionsByLectureId(int lectureId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords);
        bool IsQuestionInExam(string questionId);
        void DeleteQuestion(string questionId);
        Question? GetQuestionById(string questionId);
        string CreateQuestion(QuestionCreateViewModel model);
    }

    public class QuestionRepository : IQuestionRepository
    {
        private readonly ExamOnlineContext _context;

        public QuestionRepository(ExamOnlineContext context)
        {
            _context = context;
        }

        public List<QuestionListViewModel> GetQuestionsByLectureId(int lectureId, string searchQuery,
            string sortColumn, bool isAsc, int pageSize, int pageNumber, out int totalRecords)
        {
            var query = _context.Questions
                .Where(q => q.LectureId == lectureId && q.IsDelete != true)
                .Select(q => new QuestionListViewModel
                {
                    QuestionId = q.QuestionId,
                    Content = q.Content,
                    Type = q.Type,
                    CreatedDate = q.CreatedDate,
                    HasAnswer = q.HasAnswer,
                    IsActive = q.IsActive,
                    CanDelete = !q.ExamQuestions.Any()
                });

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim().ToLower();
                query = query.Where(q => (q.Content.ToLower().Contains(searchQuery) || q.Type.ToLower().Contains(searchQuery)));
            }

            totalRecords = query.Count();

            query = OrderListQuestion(query, sortColumn, isAsc);

            var pagedQuestions = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return pagedQuestions;
        }

        private IQueryable<QuestionListViewModel> OrderListQuestion(IQueryable<QuestionListViewModel> query,
            string? sortColumn, bool isAsc)
        {
            query = query.OrderByDescending(q => q.CreatedDate);

            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "content":
                        query = isAsc ? query.OrderBy(q => q.Content) : query.OrderByDescending(q => q.Content);
                        break;
                    case "type":
                        query = isAsc ? query.OrderBy(q => q.Type) : query.OrderByDescending(q => q.Type);
                        break;
                    case "createddate":
                        query = isAsc ? query.OrderBy(q => q.CreatedDate) : query.OrderByDescending(q => q.CreatedDate);
                        break;
                    case "hasanswer":
                        query = isAsc ? query.OrderBy(q => q.HasAnswer) : query.OrderByDescending(q => q.HasAnswer);
                        break;
                    case "isactive":
                        query = isAsc ? query.OrderBy(q => q.IsActive) : query.OrderByDescending(q => q.IsActive);
                        break;
                }
            }

            return query;
        }

        public bool IsQuestionInExam(string questionId)
        {
            return _context.ExamQuestions.Any(eq => eq.QuestionId == questionId);
        }

        public void DeleteQuestion(string questionId)
        {
            var question = _context.Questions.Find(questionId);
            question.IsDelete = true;
            _context.SaveChanges();
        }

        public Question? GetQuestionById(string questionId)
        {
            return _context.Questions
                    .Include(q => q.Options)
                    .Include(q => q.Lecture)
                    .FirstOrDefault(q => q.QuestionId == questionId);
        }

        public string CreateQuestion(QuestionCreateViewModel model)
        {
            string questionId = Guid.NewGuid().ToString();
            var question = new Question
            {
                QuestionId = questionId,
                Content = model.Content,
                Type = model.Type,
                LectureId = model.LectureID,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now,
                HasAnswer = model.Options.Any(),
                IsDelete = false
            };

            _context.Questions.Add(question);

            if (model.Options != null && model.Options.Count > 0)
            {
                foreach (var optionModel in model.Options)
                {
                    var option = new Option
                    {
                        Content = optionModel.Content,
                        IsCorrect = optionModel.IsCorrect,
                        QuestionId = questionId
                    };

                    _context.Options.Add(option);
                }
            }

            _context.SaveChanges();
            return questionId;
        }
    }
}
