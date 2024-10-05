using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class ContactPayment
    {
        [Key]
        public int Id { get; set; }  // Primary key

        [Required]
        public int ContactId { get; set; }  // Foreign key to Contact

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }  // Navigation property to Contact

        [Required]
        public int TreasuryId { get; set; }  // Foreign key to Treasury

        [ForeignKey("TreasuryId")]
        public Treasury Treasury { get; set; }  // Navigation property to Treasury

        [Required]
        public int UserId { get; set; }  // Foreign key to User who handled the payment

        [ForeignKey("UserId")]
        public User User { get; set; }  // Navigation property to User

        [Required]
        public int BranchId { get; set; }  // Foreign key to Branch where the payment occurred

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }  // Navigation property to Branch

        [Required]
        public decimal Amount { get; set; }  // Payment amount
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Notes { get; set; }  // Optional notes for the payment

        [Required]
        public PaymentType PaymentType { get; set; }  // Enum for In/Out payment (inflow or outflow)
    }
}
