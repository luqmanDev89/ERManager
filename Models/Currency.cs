using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; } 

        [Required]
        [StringLength(50)]
        public required string Name { get; set; } // Currency Name (e.g., Dollar, Euro)

        [Required]
        [StringLength(10)]
        public required string Code { get; set; } // ISO Code (e.g., USD, EUR)

        [StringLength(10)]
        public string? Symbol { get; set; } // Optional Symbol (e.g., $, €)
        public bool IsDefault { get; set; } = false;

        public ICollection<ContactPayment> ContactPayments { get; set; } = [];
        public ICollection<Expenses> Expenses { get; set; } = [];
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = [];
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = [];
    }
}
