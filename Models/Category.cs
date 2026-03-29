using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(7)]
        public string? Color { get; set; } // Hex code like #FF5733

        public decimal? MonthlyBudget { get; set; }

        public int DisplayOrder { get; set; }

        [StringLength(50)]
        public string? Group { get; set; } // e.g., Essentials, Leisure

        // Navigation property for related expenses
        public ICollection<Expense>? Expenses { get; set; }
    }
}
