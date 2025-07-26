using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERManager.Models
{
    public class Treasury
    {
        [Key]
        [DisplayName("کۆد")]
        public int TreasuryId { get; set; }

        [Required(ErrorMessage = "تکایە ناو بنووسە")]
        [StringLength(100)]
        [DisplayName("ناو")]
        public required string Name { get; set; }

        [StringLength(500)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("بەکارهێنەر")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [DisplayName("بەکارهێنەر")]
        public virtual User? User { get; set; }

        [Required]
        [DisplayName("لق")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        [DisplayName("لق")]
        public virtual Branch? Branch { get; set; }

        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>(); // Related money transactions
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>(); // Related contact payments
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>(); // Related expenses

        public virtual ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfersAsSource { get; set; } = new List<TreasuryMoneyTransfer>();
        public virtual ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfersAsDestination { get; set; } = new List<TreasuryMoneyTransfer>();
    }
}
