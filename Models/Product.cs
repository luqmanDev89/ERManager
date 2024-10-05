using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERManager.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }  // Primary key

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Name of the product, required and with a max length of 100 characters

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Buying Price must be non-negative.")]
        public decimal BuyingPrice { get; set; }  // Price at which the product is purchased, must be non-negative

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Selling Price must be non-negative.")]
        public decimal SellingPrice { get; set; }  // Price at which the product is sold, must be non-negative

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative.")]
        public int Stock { get; set; }  // Stock quantity, must be non-negative

        [StringLength(50)]
        public string Barcode { get; set; }  // Optional barcode field with a maximum length of 50 characters

        public bool IsActive { get; set; } = true;  // Whether the product is active or available
        [Required]
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }
    }
}
