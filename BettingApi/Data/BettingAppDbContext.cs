using Microsoft.EntityFrameworkCore;
using BettingApi.Models;

namespace BettingApi.Data
{
    public class BetAppDbContext : DbContext
    {
        public BetAppDbContext(DbContextOptions<BetAppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Deposit> Deposits { get; set; }

        
    }
}
