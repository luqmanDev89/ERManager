using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class TreasuryMoneyTransfer
    {
        [Key]
        [DisplayName("کۆد")]
        public int TransferId { get; set; }

        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("لە قاسەی")]
        public int SourceTreasuryId { get; set; }

        [DisplayName("تێبینی")]
        public string? Notes { get; set; }

        [ForeignKey(nameof(SourceTreasuryId))]
        [DisplayName("لە قاسەی")]
        public virtual Treasury? SourceTreasury { get; set; }

        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("بۆ قاسە")]
        public int DestinationTreasuryId { get; set; }

        [ForeignKey(nameof(DestinationTreasuryId))]
        [DisplayName("بۆ قاسەی")]
        public virtual Treasury? DestinationTreasury { get; set; }

        [Required(ErrorMessage = "تکایە بڕی پارە بنووسە")]
        [DisplayName("بڕ")]
        [Range(0, double.MaxValue, ErrorMessage = "بڕی پارە نایدانی نوسین")]
        public double Amount { get; set; } = 0;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency? Currency { get; set; }
    }
}
