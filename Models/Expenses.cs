using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class Expenses
    {
        [Key]
        public int Id { get; set; } // Primary key

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdateAt { get; set; } = DateTime.Now;


        [Required]
        public int TreasuryId { get; set; }

        [ForeignKey("TreasuryId")]
        public virtual Treasury? Treasury { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ExpensesCategory? Category { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }
    }
}
