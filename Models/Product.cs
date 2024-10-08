using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Buying Price must be non-negative.")]
        public decimal BuyingPrice { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Selling Price must be non-negative.")]
        public decimal SellingPrice { get; set; } 
        [StringLength(50)]
        public string? Barcode { get; set; } 
        public bool IsActive { get; set; } = true;


        [Required]
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory? ProductCategory { get; set; }
    }
}
