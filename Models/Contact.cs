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
    public  class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("ناو")]
        public required string Name { get; set; }

        [StringLength(500)]
        [DisplayName("ناونیشان")]
        public string? Address { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("موبایل")]
        public required string Phone { get; set; }

        [StringLength(100)]
        [DisplayName("ئیمەیڵ")]
        public string? Email { get; set; }

        [DisplayName("حاڵەت")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DisplayName("جۆری حساب")]
        public ContactType ContactType { get; set; }


        [DisplayName("بەروار")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [DisplayName("بەرواری نوێکردنەوە")]
        public DateTime? UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }
        [DisplayName("بەکارهێنەر")]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int BranchId { get; set; }
        [ForeignKey("BranchId")]
        [DisplayName("لق")]
        public virtual Branch? Branch { get; set; }

        // One-to-many relationship with ContactPayment
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>();
    }
}
