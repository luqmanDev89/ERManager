using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class SaleInvoice
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [StringLength(500)]
        [DisplayName("تێبینی")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        [DisplayName("داشکان")]
        public double? Discount { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [DisplayName("سعی")]
        public double? Tax { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [DisplayName("مەسروف")]
        public double? Expenses { get; set; } = 0;


        [Range(0, double.MaxValue)]
        [DisplayName("کرێی شۆفێر")]
        public double? DriverTax { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [DisplayName("کرێی کرێکار")]
        public double? EmployeTax { get; set; } = 0;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdateAt { get; set; }= DateTime.Now;

        [DisplayName("حاڵەت")]
        public bool? IsPaid { get; set; } = false;
        [DisplayName("کۆدی پسوولەی پارەدان")]
        public int? ContactPaymentId { get; set; }

        [Required]
        [DisplayName("بازرگان")]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        [Required]
        [DisplayName("باکارهێنەر")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; }

        // Navigation property for related invoice items
        public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItem>();

        // Calculated property for the total amount of invoice items
        [NotMapped]
        [DisplayName("کۆی پسوولە")]
        public double InvoiceItemsTotal => SaleInvoiceItems?.Sum(item => item.LineTotal) ?? 0;

        [NotMapped]
        [DisplayName("کۆی تچوو")]
        public double InvoiceItemsBuyTotal => SaleInvoiceItems?.Sum(item => item.LineBuyTotal) ?? 0;

        [NotMapped]
        [DisplayName("کۆی گشتی")]
        public double InvoiceTotal => (InvoiceItemsTotal + (Tax ?? 0) + (DriverTax ?? 0) + (EmployeTax ?? 0)+(Expenses??0)) - (Discount ?? 0);


        // Calculated property for the total profit
        [NotMapped]
        [DisplayName("کۆی قازانج")]
        public double ProfitTotal
        {
            get
            {
                double itemsLineTotal = SaleInvoiceItems?.Sum(item => item.Profit) ?? 0;
                return (itemsLineTotal + Tax??0) - Discount??0;
            }
        }
    }
}
