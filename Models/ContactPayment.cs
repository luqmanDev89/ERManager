using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public enum PaymentType
    {
        Inflow,   // Money received
        Outflow   // Money paid out
    }

    public class ContactPayment
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [Required(ErrorMessage = "تکایە بڕی پارە داخل بکە")]
        [DisplayName("بڕ")]
        public double Amount { get; set; }

        [StringLength(500)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Required]
        [DefaultValue(PaymentType.Inflow)]
        [DisplayName("جۆر")]
        public PaymentType PaymentType { get; set; }

        [Required]
        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        [DisplayName("کۆدی پسوولە")]
        public int? PurchaseInvoiceId { get; set; }

        [ForeignKey("PurchaseInvoiceId")]
        public virtual PurchaseInvoice? PurchaseInvoice { get; set; }

        [DisplayName("کۆدی پسوولە")]
        public int? PurchaseInvoiceItemId { get; set; }

        [ForeignKey("PurchaseInvoiceItemId")]
        public virtual PurchaseInvoiceItem? PurchaseInvoiceItem { get; set; }

        [DisplayName("کۆدی پسوولە")]
        public int? SaleInvoiceId { get; set; }

        [ForeignKey("SaleInvoiceId")]
        public virtual SaleInvoice? SaleInvoice { get; set; }

        [Required]
        [DisplayName("جۆری دراو")]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }

        [Required(ErrorMessage = "تکایە بازرگان دیاری بکە")]
        [DisplayName("بازرگان")]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact? Contact { get; set; }

        [Required(ErrorMessage = "تکایە قاسە دیاری بکە")]
        [DisplayName("قاسە")]
        public int TreasuryId { get; set; }

        [ForeignKey("TreasuryId")]
        public virtual Treasury? Treasury { get; set; }

        [Required]
        [DisplayName("بەکارهێنەر")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
