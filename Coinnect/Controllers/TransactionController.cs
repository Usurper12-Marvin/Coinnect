using Coinnect.Data;
using Coinnect.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using static Coinnect.Models.AllModels;

namespace Coinnect.Controllers
{
    public class TransactionController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CoinnectDbContext _coinnectDbContext;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankAccountRepository _accountRepository;

        public TransactionController(UserManager<User> userManager, CoinnectDbContext coinnectDbContext, ITransactionRepository transactionRepository, IBankAccountRepository accountRepository)
        {
            _userManager = userManager;
            _coinnectDbContext = coinnectDbContext;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Deposit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(new ListViewModel()
            {
                Accounts = await _accountRepository.GetAccountsByUserId(user.Id)
            });
        }
        [HttpPost]
        public async Task<IActionResult> Deposit(ListViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var account = _transactionRepository.GetByAccount(user, model.transfer.toAccount);
                if (account == null)
                {
                    ModelState.AddModelError("", "This account does not exist or does not belong to you.");
                    return View(model);
                }
                account.Balance += model.transfer.Amount;
                var transaction = new Transaction
                {
                    AccountId = account.AccountId,
                    UserId = user.Id,
                    TransactionType = "Deposit",
                    Description = "You have made a deposit.",
                    Amount = model.transfer.Amount,
                    Balance = account.Balance.ToString("0.00"),
                    TransactionDate = DateTime.Now,
                    toAccount = account.AccountNumber
                };

                await _transactionRepository.DepositMoney(transaction, account);

                return RedirectToAction("MyAccount", "Home");
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Transact()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            return View(new ListViewModel()
            {
                Accounts = await _accountRepository.GetAccountsByUserId(user.Id)
            });
        }
        [HttpPost]
        public async Task<IActionResult> Transact(ListViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var sourceAccount = _transactionRepository.GetByAccount(user, model.transfer.fromAccount);

                var destinationAccount = _transactionRepository.GetByAccount(user, model.transfer.toAccount);

                if (sourceAccount == null || destinationAccount == null)
                {
                    ModelState.AddModelError("", "One or both accounts do not exist or do not belong to you.");

                    return View(model);
                }
                if (sourceAccount == destinationAccount)
                {
                    ModelState.AddModelError("", "You cannot make a transaction to the same account.");
                    return View(model);
                }
                if (sourceAccount.Balance < model.transfer.Amount)
                {
                    ModelState.AddModelError("", "Insufficient balance in the source account.");
                    return View(model);
                }

                sourceAccount.Balance -= model.transfer.Amount;
                destinationAccount.Balance += model.transfer.Amount;

                var sourceTransaction = new Transaction
                {
                    AccountId = sourceAccount.AccountId,
                    UserId = user.Id,
                    Description = $"Transfer has been made to {destinationAccount.AccountNumber}",
                    TransactionType = "Debit",
                    Amount = model.transfer.Amount,
                    TransactionDate = DateTime.Now,
                    toAccount = destinationAccount.AccountNumber,
                    Balance = $"R {sourceAccount.Balance}"
                };

                var destinationTransaction = new Transaction
                {
                    AccountId = destinationAccount.AccountId,
                    UserId = user.Id,
                    Description = $"Transfer has been received from {sourceAccount.AccountNumber}",
                    TransactionType = "Credit",
                    Amount = model.transfer.Amount,
                    TransactionDate = DateTime.Now,
                    fromAccount = sourceAccount.AccountNumber,
                    Balance = $"R {destinationAccount.Balance}"
                };

                await _transactionRepository.Transfer(sourceTransaction, destinationTransaction, sourceAccount, destinationAccount);

                return RedirectToAction("MyAccount", "Home");
            }
            return View(model);
        }

        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var transactions = _transactionRepository.GetAllUsersTransactions(user.Id);

            var histroy = new ListViewModel
            {
                Transactions = transactions
            };
            return View(histroy);
        }

        public async Task<IActionResult> StatementPDF()
        {
            var user = await _userManager.GetUserAsync(User);
            var transactions = _transactionRepository.GetAllUsersTransactions(user.Id);

            var histroy = new ListViewModel
            {
                Transactions = transactions
            };

            return new ViewAsPdf("Statement", histroy)
            {
                FileName = $"Statement_{DateTime.Now:dd MMMM yyyy HH:mm:ss}.pdf"
            };
        }
    }
}

