namespace ERManager.Models
{
    public class ProductCategory
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Name of the category

        // Navigation property for the related Products
        public ICollection<Product> Products { get; set; }

        // Constructor
        public ProductCategory()
        {
            Products = new List<Product>();
        }
    }
}
