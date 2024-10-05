using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Name of the branch

        [StringLength(200)]
        public string? Location { get; set; } // Location of the branch

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Creation date
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // Last updated date
    }
}
