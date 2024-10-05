using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Treasury
    {
        [Key]
        public int TreasuryId { get; set; } // Primary key
        [Required]
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        public required Branch Branch { get; set; }
        [Required]
        public required string Name { get; set; } // Name of the treasury

        public string? Description { get; set; } // Description of the treasury

        // Foreign key to User
        [Required]
        public int UserId { get; set; } // Foreign key

        [ForeignKey("UserId")]
        public required User User { get; set; } // Navigation property

        // One-to-Many Relationship: A Contact can have multiple ContactPayments
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>();
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = new List<TreasuryMoneyTransfer>();
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>();
        public ICollection<Expenses> Expensess { get; set; } = new List<Expenses>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
