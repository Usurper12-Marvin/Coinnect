using Microsoft.EntityFrameworkCore;
using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly CoinnectDbContext _context;
        public BankAccountRepository(CoinnectDbContext context)
        {

            _context = context;

        }
        public string GenerateUniqueAccountNumber()
        {
            string accountNumber;
            bool exists;

            do
            {
                accountNumber = GenerateAccountNumber();

                exists = _context.Accounts.Any(a => a.AccountNumber == accountNumber);
            }
            while (exists);

            return accountNumber;
        }

        private string GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999).ToString();
        }

        public async Task<bool> CreateAccount(BankAccount account)
        {
            _context.Accounts.Add(account);
            //var trans = new Transaction
            //{

            //};
            //var transaction = new Transaction
            //{
            //    AccountId = account.AccountId,
            //    UserId = account.UserId,
            //    TransactionType = "Deposit",
            //    Description = "You have made a deposit.",
            //    Amount = account.Balance,
            //    Balance = account.Balance.ToString("0.00"),
            //    TransactionDate = DateTime.Now,
            //    toAccount = account.AccountNumber
            //};
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetAccountCount(string id)
        {
            return await _context.Accounts.CountAsync(c => c.UserId == id);
        }

        public async Task<IEnumerable<BankAccount>> GetAccountsByUserId(string id)
        {
            return await _context.Accounts.Where(a => a.UserId == id).ToListAsync();
        }

        public BankAccount GetAccountById(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == id);
        }

        public void GetTransactionsByUserId(string id)
        {
            _context.Transactions.Where(t => t.UserId == id).ToList();
        }
    }
}
