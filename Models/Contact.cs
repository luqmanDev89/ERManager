using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{

    public abstract class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public required string Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign key to User (the user who manages the person)
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public required User User { get; set; } // Navigation property for the associated user
        [Required]
        public int BranchId { get; set; } // Foreign key to Branch

        [ForeignKey("BranchId")]
        public required Branch Branch { get; set; } // Navigation property for Branch
        // Add PersonType property
        [Required]
        public ContactType ContactType { get; set; } // Uses the PersonType enum
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
