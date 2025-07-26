using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERManager.Models
{
    public class PurchaseInvoiceItem
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }  // Item ID

        [Required]
        [DisplayName("کۆدی پسوولە")]
        public int PurchaseInvoiceId { get; set; }
        [ForeignKey("PurchaseInvoiceId")]
        public virtual PurchaseInvoice PurchaseInvoice { get; set; }

        [Required]
        [DisplayName("کاڵا")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [DisplayName("کاڵا")]
        public virtual Product Product { get; set; }

        [Required]
        [DisplayName("بڕ")]
        public double Quantity { get; set; } = 0;

        [Required]
        [DisplayName("نرخ")]
        public double UnitPrice { get; set; } = 0;


        [DisplayName("گومرك")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TaxPresent { get; set; } = 0;

        [DisplayName("حاڵەت")]
        public bool? IsPaid { get; set; } = false;

        [DisplayName("کۆدی پسوولەی پارەدان")]
        public int? ContactPaymentId { get; set; }

        [DisplayName("کۆ")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double LineTotal => (Quantity * UnitPrice) * (1 + (TaxPresent / 100));


       
    }

}