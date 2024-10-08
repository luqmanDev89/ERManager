using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public enum TransactionType
    {
        In,  // Inflow
        Out  // Outflow
    }

    public class MoneyTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Notes { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; } = TransactionType.In;  // Enum type for transaction direction

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public int TreasuryId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury? Treasury { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }

    }

}
