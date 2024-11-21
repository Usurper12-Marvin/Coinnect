using static Coinnect.Models.AllModels;
using static Coinnect.Models.ViewModels.UserViewModels;

namespace Coinnect.Models.ViewModels
{
    public class ListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public User theUser { get; set; }
        public Task<IList<User>> Clients { get; set; }
        public IEnumerable<Advice> Advices { get; set; }
        public IEnumerable<BankAccount> Accounts { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public Transfer transfer { get; set; }
        public IEnumerable<ActionLog> Actions { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<ContactUs> Messages { get; set; }
    }
}
