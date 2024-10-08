using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class TreasuryMoneyTransfer
    {
        [Key]
        public int TransferId { get; set; } 

        [Required]
        public int SourceTreasuryId { get; set; }


        public string? Notes { get; set; }

        [ForeignKey("SourceTreasuryId")]
        public virtual Treasury? SourceTreasury { get; set; } 

        [Required]
        public int DestinationTreasuryId { get; set; } 

        [ForeignKey("DestinationTreasuryId")]
        public virtual Treasury? DestinationTreasury { get; set; } 

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
        public decimal Amount { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now; 
        public DateTime UpdateAt { get; set; } = DateTime.Now; 



        [Required]
        public int UserId { get; set; } 

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency? Currency { get; set; } 
    }
}
