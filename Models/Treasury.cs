using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class Treasury
    {
        [Key]
        public int TreasuryId { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public required string Name { get; set; } // Name of the treasury

        [StringLength(500)]
        public string? Description { get; set; } // Optional description

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Creation date
        public DateTime UpdateAt { get; set; } = DateTime.Now; // Last updated date


        [Required]
        public int UserId { get; set; } // User managing the treasury

        [ForeignKey("UserId")]
        public virtual User? User { get; set; } // User associated with the treasury

        [Required]
        public int BranchId { get; set; } // Branch associated with the treasury

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; } // Branch associated with the treasury


        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>(); // Related money transactions
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>(); // Related contact payments
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>(); // Related expenses
                                                                                    // Navigation properties for TreasuryMoneyTransfer
        public virtual ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfersAsSource { get; set; } = new List<TreasuryMoneyTransfer>();
        public virtual ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfersAsDestination { get; set; } = new List<TreasuryMoneyTransfer>();
    }
}
