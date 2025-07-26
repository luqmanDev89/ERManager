using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DisplayName("کۆد")]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "تکایە بڕی پارە داخل بکە")]
        [DisplayName("بڕ")]
        public double Amount { get; set; } = 0;

        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [Required]
        [DisplayName("جۆر")]
        public TransactionType TransactionType { get; set; } = TransactionType.In;  // Enum type for transaction direction

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DisplayName("نوێکردنوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("قاسە")]
        public int TreasuryId { get; set; }
        [DisplayName("قاسە")]
        [ForeignKey("TreasuryId")]
        public virtual Treasury? Treasury { get; set; }

        [Required]
        [DisplayName("بەکارهێنەر")]
        public string UserId { get; set; }
        [DisplayName("بەکارهێنەر")]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }

    }

}
