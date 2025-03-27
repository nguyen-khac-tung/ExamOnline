using ExamationOnline.Models;
using ExamationOnline.ViewModels;

namespace ExamationOnline.Repository
{
    public interface IUserRepository
    {
        public User? GetAccount(AccountLoginVM accountLogin);
        public User GetUserById(int userId);
        public User? GetUserByEmail(string email);
        public bool CheckEmailExist(User user);
        public void AddUser(AccountResgisterVM accountResgister);
    }

    public class UserRepository : IUserRepository
    {
        private readonly ExamOnlineContext _context;
        
        public UserRepository(ExamOnlineContext dbContext)
        {
            _context = dbContext;
        }

        public User? GetAccount(AccountLoginVM accountLogin)
        {
            var account = (from a in _context.Users
                           where a.Email == accountLogin.Email
                                        && a.Password == accountLogin.Password
                           select new User()
                           {
                               Email = a.Email,
                               FullName = a.FullName,
                               Role = a.Role,
                           }).FirstOrDefault();
            return account;
        }

        public User GetUserById(int userId)
        {
            var user = (from a in _context.Users
                           where a.UserId == userId
                           select new User()
                           {
                               UserId = a.UserId,
                              FullName  = a.FullName,
                               Email = a.Email,
                               Password = a.Password,
                               Role = a.Role,
                               IsDelete = a.IsDelete,
                           }).FirstOrDefault();
            return user;
        }

        public User? GetUserByEmail(string email)
        {
            var user = (from a in _context.Users
                           where a.Email == email
                           select new User()
                           {
                               UserId = a.UserId,
                               FullName = a.FullName,
                               Email = a.Email,
                               Role = a.Role,
                           }).FirstOrDefault();
            return user;
        }

        public bool CheckEmailExist(User user)
        {
            var acc = (from a in _context.Users
                       where a.UserId != user.UserId && a.Email.ToLower() == user.Email.ToLower()
                       select a).FirstOrDefault();
            return acc != null ? true : false;
        }

        public void AddUser(AccountResgisterVM accountResgister)
        {
            User newUser = new User();
            newUser.FullName = accountResgister.FullName;
            newUser.Email = accountResgister.Email;
            newUser.Password = accountResgister.Password;
            newUser.Role = accountResgister.Role;
            newUser.IsDelete = false;

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
