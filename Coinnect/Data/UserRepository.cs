using Microsoft.AspNetCore.Identity;
using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        protected CoinnectDbContext Context;
        public UserRepository(CoinnectDbContext identityContext, UserManager<User> userManager)
        {
            _userManager = userManager;
            this.Context = identityContext;

        }
        async Task<User> IUserRepository.GetById(string id)
        {

            return await _userManager.FindByIdAsync(id);
        }
        public async Task DeleteA(string id)
        {
            var userBankAccounts = Context.Accounts
                .Where(b => b.UserId == id)
                .ToList();
            foreach (var account in userBankAccounts)
            {
                var transactions = Context.Transactions
                    .Where(t => t.AccountId == account.AccountId)
                    .ToList();

                Context.Transactions.RemoveRange(transactions);
            }
            Context.Accounts.RemoveRange(userBankAccounts);
            await Context.SaveChangesAsync();
        }
    }
}
