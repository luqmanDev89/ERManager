using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public string Username { get; set; } // Username for login

        [Required]
        [StringLength(100)]
        public string Password { get; set; } // Password (consider using a hashed password in production)

        [Required]
        [StringLength(50)]
        public string Role { get; set; } // User role (e.g., Admin, User)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Creation date
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // Last updated date

        // One-to-Many Relationship: A Contact can have multiple ContactPayments
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>();
        public ICollection<Treasury> Treasury { get; set; } = new List<Treasury>();
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = new List<TreasuryMoneyTransfer>();
    }
}
