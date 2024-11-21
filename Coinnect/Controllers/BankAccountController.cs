using Coinnect.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Coinnect.Models.AllModels;
using static Coinnect.Models.ViewModels.UserViewModels;

namespace Coinnect.Controllers
{
    public class BankAccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IBankAccountRepository _bankaccount;
        private readonly ITransactionRepository _transactionRepository;

        public BankAccountController(UserManager<User> userManager, IBankAccountRepository bankaccount, ITransactionRepository transactionRepository)
        {
            _userManager = userManager;
            _bankaccount = bankaccount;
            _transactionRepository = transactionRepository;
        }

        public IActionResult Details(int id)
        {
            var account = _bankaccount.GetAccountById(id);
            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var accountCount = await _bankaccount.GetAccountCount(user.Id);
            if (accountCount >= 4)
            {
                ModelState.AddModelError("", "You have already reached the maximum of 4 accounts.");

                TempData["error"] = "You have exceeded the number of accounts you can create(4).";
                return RedirectToAction("MyAccount", "Home");
            }

            return View(new CreateAccount());
        }

        // POST: /Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(CreateAccount model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var accountNumber = _bankaccount.GenerateUniqueAccountNumber();
                var account = new BankAccount
                {
                    AccountNumber = accountNumber,
                    AccountType = model.AccountType,
                    Balance = model.InitialDeposit,
                    UserId = user.Id

                };

                var result = await _bankaccount.CreateAccount(account);
                if (result)
                {
                    var transaction = new Transaction
                    {
                        AccountId = account.AccountId,
                        UserId = user.Id,
                        TransactionType = "Deposit",
                        Description = "An account was created along with a deposit.",
                        Amount = model.InitialDeposit,
                        Balance = account.Balance.ToString("0.00"),
                        TransactionDate = DateTime.Now,
                        toAccount = account.AccountNumber
                    };

                    await _transactionRepository.DepositMoney(transaction, account);
                    TempData["message"] = "You have successfully created and account";
                    return RedirectToAction("MyAccount", "Home");
                }

                ModelState.AddModelError("", "Failed to create account.");
            }

            return View(model);
        }
    }
}
