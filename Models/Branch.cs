using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Branch
    {

        [DisplayName("کۆد")]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("ناو")]
        public required string Name { get; set; }

        [StringLength(200)]
        [DisplayName("ناونیشان")]
        public string? Location { get; set; } = string.Empty;

        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDefault { get; set; } = false;

        // Navigation Properties
        [DisplayName("بازرگان")]
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        [DisplayName("مەسروفات")]
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();
        [DisplayName("قاسەکان")]
        public ICollection<Treasury> Treasuries { get; set; } = new List<Treasury>();
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
    }
}
