using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ERManager.Models
{

    public class ExpensesCategory
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("ناو")]
        public required string Name { get; set; }
        public ICollection<Expenses> Expenses { get; set; } = [];
    }
}
