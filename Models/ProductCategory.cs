using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class ProductCategory
    {
        [DisplayName("کۆد")]
        [Key]
        public int Id { get; set; }
        [DisplayName("ناو")]
        [Required]
        public required string Name { get; set; }


        public ICollection<Product> Products { get; set; } = [];
    }
}
