using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public enum UserRole
    {
        Admin,
        User,
        Manager,
        // Add other roles as needed
    }

    public class User
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public required string Username { get; set; } // Unique username for the user

        [Required]
        [StringLength(100)]
        [PasswordPropertyText]
        public required string Password { get; set; } // Hashed password for authentication

        [Required]
        public UserRole Role { get; set; } // User role (using enum for type safety)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Creation timestamp
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // Last update timestamp

        // Navigation properties
        public ICollection<ContactPayment> ContactPayments { get; set; } = new List<ContactPayment>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();
        public ICollection<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>();
        public ICollection<Treasury> Treasuries { get; set; } = new List<Treasury>();
        public ICollection<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = new List<TreasuryMoneyTransfer>();
    }
}
