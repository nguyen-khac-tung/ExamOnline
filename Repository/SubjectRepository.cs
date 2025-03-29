using ExamationOnline.Models;

namespace ExamationOnline.Repository
{
    public interface ISubjectRepository
    {
        List<Subject> GetListSubject();
    }

    public class SubjectRepository : ISubjectRepository
    {
        private readonly ExamOnlineContext _context;

        public SubjectRepository(ExamOnlineContext context)
        {
            _context = context;
        }

        public List<Subject> GetListSubject()
        {
            return _context.Subjects.Where(c => c.IsDelete == false).ToList();
        }
    }
}
