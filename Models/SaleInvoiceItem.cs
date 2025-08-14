using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class SaleInvoiceItem
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }  // Item ID

        [Required]
        [DisplayName("کۆدی پسوولە")]
        public int SaleInvoiceId { get; set; }
        [ForeignKey("SaleInvoiceId")]
        public virtual SaleInvoice SaleInvoice { get; set; }

        [Required]
        [DisplayName("کاڵا")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [DisplayName("بڕ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double Quantity { get; set; } = 0;

        [Required]
        [DisplayName("نرخ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double UnitPrice { get; set; } = 0;

        [Required]
        [DisplayName("نرخ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double UnitBuyPrice { get; set; } = 0;

        [DisplayName("گومرك")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TaxPresent { get; set; } = 0;

        
        [DisplayName("فڕۆشیار")]
        public int? ContactId { get; set; } // Renamed for clarity
        [ForeignKey("ContactId")]
        public virtual Contact? Contact { get; set; }

        public bool IsAddedToPurchase { get; set; } = false;

        [DisplayName("کۆ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double LineTotal => (Quantity * UnitPrice) * (1 + (TaxPresent / 100));

        [DisplayName("کۆ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double LineBuyTotal => Quantity * UnitBuyPrice;

        [DisplayName("قازانج")]
        public double Profit => LineTotal - (LineBuyTotal + ((Quantity * UnitPrice) * (1 + (TaxPresent / 100))) - ((Quantity * UnitPrice)));

        public int PurchaseItemId { get; set; } = 0; // Foreign key to PurchaseItem if applicable
    }
}
