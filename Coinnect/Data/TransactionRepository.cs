using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly CoinnectDbContext _context;
        public TransactionRepository(CoinnectDbContext context)
        {

            _context = context;

        }

        public async Task<bool> DepositMoney(Transaction _transaction, BankAccount _account)
        {
            _context.Transactions.Add(_transaction);
            _context.Accounts.Update(_account);

            return await _context.SaveChangesAsync() > 0;
        }

        public BankAccount GetByAccount(User user, string accountNumber)
        {

            return _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber
            && a.UserId == user.Id);
        }

        public IEnumerable<Transaction> GetAllUsersTransactions(string id)
        {
            return _context.Transactions.Where(t => t.UserId == id).ToList();
        }

        public async Task<bool> Transfer(Transaction sourceT, Transaction destinationT, BankAccount sourceBA, BankAccount destinationBA)
        {
            _context.Transactions.Add(sourceT);
            _context.Transactions.Add(destinationT);
            _context.Accounts.Update(sourceBA);
            _context.Accounts.Update(destinationBA);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
