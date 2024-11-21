using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public interface IBankAccountRepository
    {
        Task<bool> CreateAccount(BankAccount account);
        Task<int> GetAccountCount(string id);
        Task<IEnumerable<BankAccount>> GetAccountsByUserId(string id);
        void GetTransactionsByUserId(string id);
        BankAccount GetAccountById(int id);
        string GenerateUniqueAccountNumber();
    }
}
