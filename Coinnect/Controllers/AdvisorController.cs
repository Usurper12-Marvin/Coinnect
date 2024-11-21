using Coinnect.Data;
using Coinnect.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Coinnect.Models.AllModels;
using static Coinnect.Models.ViewModels.UserViewModels;

namespace Coinnect.Controllers
{
    public class AdvisorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CoinnectDbContext _coinnectDbContext;
        private readonly ITransactionRepository _transactionRepository;
        public AdvisorController(CoinnectDbContext coinnectDbContext, UserManager<User> userManager, ITransactionRepository transactionRepository)
        {
            _coinnectDbContext = coinnectDbContext;
            _userManager = userManager;
            _transactionRepository = transactionRepository;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ClientList()
        {
            var clients = _userManager.GetUsersInRoleAsync("Client");
            var userList = new ListViewModel
            {
                Clients = clients
            };
            return View(userList);
        }
        public IActionResult ClientDetails(string id)
        {
            var user = _coinnectDbContext.Users.FirstOrDefault(a => a.Id == id);
            return View(user);
        }
        public async Task<IActionResult> UsersHistory(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var transactions = _transactionRepository.GetAllUsersTransactions(user.Id);

            var histroy = new ListViewModel
            {
                theUser = user,
                Transactions = transactions
            };
            return View(histroy);
        }
        [HttpGet]
        public IActionResult GiveAdvice(string id)
        {
            return View(new AdviceViewModel { ClientId = id });
        }

        [HttpPost]
        public async Task<IActionResult> GiveAdvice(AdviceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.GetUserId(User);
                var advice = new Advice
                {
                    ClientId = model.ClientId,
                    AdvisorId = user,
                    AdviceText = model.AdviceText,
                    DateGiven = DateTime.Now
                };
                _coinnectDbContext.Advices.Add(advice);
                await _coinnectDbContext.SaveChangesAsync();
                return RedirectToAction("ClientList");
            }
            return View(model);
        }
    }
}
