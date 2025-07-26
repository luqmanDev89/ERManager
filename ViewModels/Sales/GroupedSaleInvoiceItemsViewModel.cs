using ERManager.Models;

namespace ERManager.ViewModels.Sales
{
    public class GroupedSaleInvoiceItemsViewModel
    {
        public int? ContactId { get; set; }
        public int? SaleInvoiceId { get; set; }
        public Contact Contact { get; set; }
        public List<SaleInvoiceItemViewModel> Items { get; set; }
    }

    public class SaleInvoiceItemViewModel
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public bool IsAddedToPurchase { get; set; }
    }


}
