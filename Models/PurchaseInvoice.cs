using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERManager.Models
{
    public class PurchaseInvoice
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [DisplayName("فاکتۆر")]
        public string? InvoiceNumber { get; set; } = string.Empty;

        [StringLength(500)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [DisplayName("داشکان")]
        public double? Discount { get; set; } = 0;

        [DisplayName("مەسروف")]
        public double? Tax { get; set; } = 0;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdateAt { get; set; } = DateTime.Now;

        [DisplayName("حاڵەت")]
        public bool? IsPaid { get; set; } = false;

        [DisplayName("کۆدی پسوولەی پارەدان")]
        public int? ContactPaymentId { get; set; }

        [Required]
        [DisplayName("بازرگان")]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        [DisplayName("بازرگان")]
        public virtual Contact? Contact { get; set; } = null!; // Assume Contact is non-null when required

        [Required]
        [DisplayName("باکارهێنەر")]
        public string UserId { get; set; } = null!;

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

        // Navigation property for related invoice items
        public virtual ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();

        // Calculated property for the total items amount
        [NotMapped]
        [DisplayName("کۆی فاکتۆر")]
        public double InvoiceItemsTotal => PurchaseInvoiceItems.Sum(item => item.LineTotal);

        // Calculated property for the total invoice amount, considering tax and discount
        [NotMapped]
        [DisplayName("کۆی فاکتۆر")]
        public double InvoiceTotal => InvoiceItemsTotal + (Tax ?? 0) - (Discount ?? 0);
    }
}
