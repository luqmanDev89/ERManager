using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDefault { get; set; } = false;

        // Navigation Properties
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();
        public ICollection<Treasury> Treasuries { get; set; } = new List<Treasury>();
    }
}
