using ExamationOnline.Repository;
using ExamationOnline.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExamationOnline.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AccountLoginVM accountLogin)
        {
            if (!ModelState.IsValid) return View();

            var account = _userRepository.GetAccount(accountLogin);

            if (account == null) { ViewBag.Message = "Incorrect login information."; return View(); }
            if (account?.IsDelete == true) { ViewBag.Message = "This account has been disabled."; return View(); }

            HttpContext.Session.SetString("UserEmail", account.Email);
            HttpContext.Session.SetString("UserName", account.FullName);
            HttpContext.Session.SetString("UserRole", account.Role);

            if (account.Role == "Lecture") { return RedirectToAction("List", "Question", new { area = "Lecture" }); }
            if (account.Role == "Student") { return RedirectToAction("List", "Exam", new { area = "Student" }); }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AccountResgisterVM accountResgister)
        {
            var user = _userRepository.GetUserByEmail(accountResgister.Email);
            if (user != null) { ViewBag.Message = "Email already used to other account."; return View(); }

            accountResgister.Role = "Student";
            _userRepository.AddUser(accountResgister);

            HttpContext.Session.SetString("UserEmail", accountResgister.Email);
            HttpContext.Session.SetString("UserRole", accountResgister.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
