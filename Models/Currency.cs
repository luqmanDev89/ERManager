using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string? Code { get; set; } = "دینار";

        [StringLength(10)]
        public string? Symbol { get; set; } = "د.ع";
        public bool IsDefault { get; set; } = false;

        public ICollection<ContactPayment> ContactPayments { get; set; } = [];
        public ICollection<Expenses> Expenses { get; set; } = [];
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = [];
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = [];
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
        public ICollection<SaleInvoice> SaleInvoices { get; set; } = new List<SaleInvoice>();
    }
}
