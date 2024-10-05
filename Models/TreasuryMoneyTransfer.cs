using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class TreasuryMoneyTransfer
    {
        [Key]
        public int TransferId { get; set; } // Primary key

        [Required]
        public int SourceTreasuryId { get; set; } // Foreign key to source treasury

        [ForeignKey("SourceTreasuryId")]
        public required Treasury SourceTreasury { get; set; } // Navigation property to the source treasury

        [Required]
        public int DestinationTreasuryId { get; set; } // Foreign key to destination treasury

        [ForeignKey("DestinationTreasuryId")]
        public required Treasury DestinationTreasury { get; set; } // Navigation property to the destination treasury

        [Required]
        public decimal Amount { get; set; } // Amount of money being transferred


        [Required]
        public int UserId { get; set; } // Foreign key to the User who initiates the transfer

        [ForeignKey("UserId")]
        public required User User { get; set; } // Navigation property to the User

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
