using System.ComponentModel.DataAnnotations;
namespace ERManager.Models
{

    public class ExpensesCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public ICollection<Expenses> Expenses { get; set; } = [];
    }
}
