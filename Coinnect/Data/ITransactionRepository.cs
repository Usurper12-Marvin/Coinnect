using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public interface ITransactionRepository
    {
        public BankAccount GetByAccount(User user, string model);
        public Task<bool> Transfer(Transaction sourceT, Transaction destinationT, BankAccount sourceBA, BankAccount destinationBA);
        public Task<bool> DepositMoney(Transaction _transaction, BankAccount _account);
        IEnumerable<Transaction> GetAllUsersTransactions(string id);
    }
}
