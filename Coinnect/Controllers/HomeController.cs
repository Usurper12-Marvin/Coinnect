using Coinnect.Data;
using Coinnect.Models;
using Coinnect.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Coinnect.Models.AllModels;

namespace Coinnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IBankAccountRepository _account;
        private readonly CoinnectDbContext _dbContext;

        public HomeController(UserManager<User> userManager, IBankAccountRepository account, ILogger<HomeController> logger, CoinnectDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _account = account;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var accounts = await _account.GetAccountsByUserId(user.Id);
            var model = new ListViewModel
            {
                Accounts = accounts
            };

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(User model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Identifier = model.Identifier;
                user.PhoneNumber = model.PhoneNumber;
                user.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitMessage(string name, string email, string message)
        {
            var contactUs = new ContactUs
            {
                ContactUsText = message,
                ClientEmail = email,
                ClientName = name,
                DateSent = DateTime.Now
            };
            _dbContext.Messages.Add(contactUs);
            _dbContext.SaveChanges();
            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction("ContactUs");
        }

    }
}
