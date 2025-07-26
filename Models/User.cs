using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class User : IdentityUser // Inherit from IdentityUser
    {
        // The Id property is already provided by IdentityUser, so you don't need to redefine it.

        [Required]
        [StringLength(100)]
        public override string UserName { get; set; } // Unique username for the user

        // Removed custom enum for role, since we're using ASP.NET Identity roles

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Creation timestamp

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // Last update timestamp

        // Navigation properties
        public ICollection<ContactPayment> ContactPayments { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Expenses> Expenses { get; set; }
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; }
        public ICollection<Treasury> Treasuries { get; set; }
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; }
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        public ICollection<SaleInvoice> SaleInvoices { get; set; }

        public User()
        {
            ContactPayments = new List<ContactPayment>();
            Contacts = new List<Contact>();
            Expenses = new List<Expenses>();
            MoneyTransactions = new List<MoneyTransaction>();
            Treasuries = new List<Treasury>();
            TreasuryMoneyTransfers = new List<TreasuryMoneyTransfer>();
            PurchaseInvoices = new List<PurchaseInvoice>();
            SaleInvoices = new List<SaleInvoice>();
        }
    }
}
