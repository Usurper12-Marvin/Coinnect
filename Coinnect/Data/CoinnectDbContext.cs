using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Coinnect.Models.AllModels;

namespace Coinnect.Data
{
    public class CoinnectDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
    {
        public DbSet<BankAccount> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> _Users { get; set; }
        public DbSet<ActionLog> actionLogs { get; set; }
        public DbSet<Advice> Advices { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ContactUs> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<BankAccount>().ToTable("BankAccounts");
            modelBuilder.Entity<Transaction>().ToTable("Transections");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<ActionLog>().ToTable("ActionLogs");
            modelBuilder.Entity<Advice>().ToTable("Advices");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<ContactUs>().ToTable("Messages");

        }
    }
}
