namespace ERManager.ViewModels.Products
{
    public class ProductStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double PurchaseQuantity { get; set; }
        public double SaleQuantity { get; set; }
        public double NetStock { get; set; }
        public int? CategoryId { get; set; } // Add this lin
    }
}
