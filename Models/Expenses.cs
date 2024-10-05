using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Expenses
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public int CategoryId { get; set; } // Foreign key to ExpensesCategory

        [ForeignKey("CategoryId")]
        public required ExpensesCategory Category { get; set; } // Navigation property
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public required User User { get; set; }

        [Required]
        public int BranchId { get; set; } // Foreign key to Branch

        [ForeignKey("BranchId")]
        public required Branch Branch { get; set; } // Navigation property for Branch

        [StringLength(200)]
        public string? Description { get; set; } // Description of the expense

        [Required]
        public decimal Amount { get; set; } // Amount of the expense
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
