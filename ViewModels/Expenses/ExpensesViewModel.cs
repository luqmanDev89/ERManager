using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.Expenses
{
    public class ExpensesViewModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        [DisplayName("توصیف")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "تکایە بڕی مەسروف داخل بکە")]
        [Range(0.01, double.MaxValue, ErrorMessage = "پێویستە لە سفر زیاتر بێت")]
        [DisplayName("بڕ")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("قاسە")]
        public int TreasuryId { get; set; }

        [Required(ErrorMessage = "تکایە جۆری مەسروف دیاری بکە")]
        [DisplayName("جۆر")]
        public int CategoryId { get; set; }

        public int? UserId { get; set; } // Optional UserId, can be filled by controller logic

        public int? BranchId { get; set; } // Optional BranchId, can be filled by controller logic

        public int? CurrencyId { get; set; } // Optional CurrencyId, can be filled by controller logic

        [DisplayName("بەروار")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("بەرواری نوێکردنەوە")]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
