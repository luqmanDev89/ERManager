using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERManager.ViewModels.Treasury
{
    public class TreasuriesActivityViewModel
    {
        [DisplayName("کۆد")]
        public int TreasuryId { get; set; } = 0;

        [StringLength(100)]
        [DisplayName("ناو")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [DisplayName("بڕ")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Amount { get; set; } = 0;

        [StringLength(100)]
        [DisplayName("بەکارهێنەر")]
        public string UserName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
