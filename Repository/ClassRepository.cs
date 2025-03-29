using ExamationOnline.Models;

namespace ExamationOnline.Repository
{
    public interface IClassRepository
    {
        List<Class> GetListClass();
    }

    public class ClassRepository : IClassRepository
    {
        private readonly ExamOnlineContext _context;

        public ClassRepository(ExamOnlineContext context)
        {
            _context = context;
        }

        public List<Class> GetListClass()
        {
            return _context.Classes.Where(c => c.IsDelete == false).ToList();
        }
    }
}
