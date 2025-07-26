using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public enum ContactType
    {
        Customer,
        Supplier,
        Both
    }
    public class Contact
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("ناو")]
        public required string Name { get; set; }

        [StringLength(500)]
        [DisplayName("ناونیشان")]
        public string? Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [DisplayName("موبایل")]
        [DataType(DataType.PhoneNumber)]
        public required string Phone { get; set; }

        [StringLength(100)]
        [DisplayName("ئیمەیڵ")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; } = string.Empty;

        [DisplayName("حاڵەت")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DisplayName("جۆری حساب")]
        public ContactType ContactType { get; set; }


        [DisplayName("دروستکردن")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("نوێکردنەوە")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdateAt { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("بەکارهێنەر")]
        public string UserId { get; set; }
        [DisplayName("بەکارهێنەر")]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        [DisplayName("لق")]
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        [DisplayName("لق")]
        public virtual Branch? Branch { get; set; }

        // One-to-many relationship with ContactPayment
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>();
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
        public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItem>();
    }
}
