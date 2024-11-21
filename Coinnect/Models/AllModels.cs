using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Coinnect.Models
{
    public class AllModels
    {

        public class User : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Identifier { get; set; }
            public string UserType { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public string UsersPassword { get; set; }
            public IEnumerable<BankAccount> BankAccounts { get; set; }
        }

        public class BankAccount
        {
            [Key]
            public int AccountId { get; set; }
            public string AccountNumber { get; set; }
            public string AccountType { get; set; }
            public decimal Balance { get; set; } = 0.00m;
            public string UserId { get; set; }
            public User User { get; set; }
        }

        public class Transaction
        {
            [Key]
            public int TransactionId { get; set; }
            public int AccountId { get; set; }
            public string UserId { get; set; }
            public string Description { get; set; }
            public string TransactionType { get; set; }
            public string toAccount { get; set; }
            public string fromAccount { get; set; }
            public string Balance { get; set; }
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; }
            public User user { get; set; }
            public BankAccount account { get; set; }
        }

        public class ActionLog
        {
            [Key]
            public int ConsultantId { get; set; }
            public string ActionPerformed { get; set; }
            public string TargetUserId { get; set; }
            public DateTime ActionDate { get; set; }
            public string Details { get; set; }
            public string UserId { get; set; }
            public User User { get; set; }
        }
        public class Advice
        {
            [Key]
            public int AdviceId { get; set; }
            public string ClientId { get; set; }
            public string AdvisorId { get; set; }
            public string AdviceText { get; set; }
            public DateTime DateGiven { get; set; }
            public virtual User Advisor { get; set; }
            public virtual User Client { get; set; }
        }
        public class Review
        {
            public int ReviewId { get; set; }
            public string UserId { get; set; }
            public int Rating { get; set; }
            public string ReviewText { get; set; }
            public DateTime DatePosted { get; set; }
            public virtual User Client { get; set; }
        }
        public class ContactUs
        {
            [Key]
            public int ContactUsId { get; set; }
            public string ContactUsText { get; set; }
            public DateTime DateSent { get; set; }
            public string ClientName { get; set; }
            public string ClientEmail { get; set; }

        }

    }
}
