using System.ComponentModel.DataAnnotations;
namespace ERManager.ViewModels.Treasury
{
    public class TreasuryViewModel
    {
        [Required]
        public int TreasuryId { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

