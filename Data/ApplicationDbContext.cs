using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed default categories with Colors, Groups, and DisplayOrders
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food", Color = "#71B37C", Group = "Essentials", DisplayOrder = 1, MonthlyBudget = 5000 },
                new Category { Id = 2, Name = "Bills", Color = "#FF6384", Group = "Essentials", DisplayOrder = 2, MonthlyBudget = 10000 },
                new Category { Id = 3, Name = "Transport", Color = "#36A2EB", Group = "Essentials", DisplayOrder = 3, MonthlyBudget = 3000 },
                new Category { Id = 4, Name = "Entertainment", Color = "#9966FF", Group = "Leisure", DisplayOrder = 4, MonthlyBudget = 4000 },
                new Category { Id = 5, Name = "Others", Color = "#E7E9ED", Group = "Misc", DisplayOrder = 5, MonthlyBudget = 1000 }
            );
        }
    }
}
