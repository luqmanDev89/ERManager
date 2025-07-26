using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class Expenses
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [StringLength(200)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "تکایە بڕی پارە داخل بکە")]
        [DisplayName("بڕ")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public double Amount { get; set; } = 0;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("قاسە")]
        public int TreasuryId { get; set; }

        [ForeignKey("TreasuryId")]
        [DisplayName("قاسە")]
        public virtual Treasury? Treasury { get; set; }

        [Required(ErrorMessage = "تکایە جۆری مەسروف دیاری بکە")]
        [DisplayName("جۆر")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [DisplayName("جۆر")]
        public virtual ExpensesCategory? Category { get; set; }

        [Required]
        [DisplayName("بەکارهێنەر")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [DisplayName("بەکارهێنەر")]
        public virtual User? User { get; set; }

        [Required]
        [DisplayName("لق")]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        [DisplayName("لق")]
        public virtual Branch? Branch { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }
    }
}
