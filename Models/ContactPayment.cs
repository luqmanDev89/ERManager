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
        public int Id { get; set; }  
        [Required]
        public decimal Amount { get; set; } 
        [StringLength(500)]
        public string? Notes { get; set; }
        [Required]
        public PaymentType PaymentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;


        [Required]
        public int CurrencyId { get; set; } 
        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; } 

        [Required]
        public int ContactId { get; set; } 

        [ForeignKey("ContactId")]
        public virtual Contact? Contact { get; set; }  

        [Required]
        public int TreasuryId { get; set; } 

        [ForeignKey("TreasuryId")]
        public virtual Treasury? Treasury { get; set; }  

        [Required]
        public int UserId { get; set; }  

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }  

    }
}
