using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class MoneyTransaction
    {
        [Key]
        public int TransactionId { get; set; } // Primary key

        [Required]
        public decimal Amount { get; set; } // Amount of money

        [Required]
        public required string TransactionType { get; set; } = "In"; // 'In' or 'Out'

        [Required]
        public int TreasuryId { get; set; } // Foreign key to Treasury

        [ForeignKey("TreasuryId")]
        public required Treasury Treasury { get; set; } // Navigation property

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [ForeignKey("UserId")]
        public required User User { get; set; } // Navigation property

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
