using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERManager.Models
{
    public class Product
    {
        [Key]
        [DisplayName("کۆد")]
        public int Id { get; set; }

        [Required(ErrorMessage = "ناوی کاڵا بنووسە")]
        [StringLength(100)]
        [DisplayName("ناو")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "تکایە نرخی کڕین بنووسە")]
        [DisplayName("نرخی کڕین")]
        [Range(0, double.MaxValue, ErrorMessage = "Buying Price must be non-negative.")]
        public decimal BuyingPrice { get; set; } = 0;
        [Required(ErrorMessage = "تکایە نرخی فڕۆشتن بنووسە")]
        [DisplayName("نرخی فڕۆشتن")]
        [Range(0, double.MaxValue, ErrorMessage = "Selling Price must be non-negative.")]
        public decimal SellingPrice { get; set; } = 0;
        [StringLength(50)]
        [DisplayName("بارکۆد")]
        public string? Barcode { get; set; } = string.Empty;
        [DisplayName("حاڵەت")]
        public bool IsActive { get; set; } = true;


        [Required]
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public virtual ProductCategory? ProductCategory { get; set; }
    }
}
