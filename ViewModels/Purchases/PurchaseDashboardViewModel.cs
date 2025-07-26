using ERManager.Models;

namespace ERManager.ViewModels.Purchases
{
    public class PurchaseDashboardViewModel
    {
        public IEnumerable<PurchaseInvoice> Last20Purchases { get; set; } = new List<PurchaseInvoice>();
        public IEnumerable<ContactPayment> Last20ContactPayments { get; set; } = new List<ContactPayment>();
        public double TotalSupplierDebit { get; set; } = 0;
        public double ToDayTotalPurchase { get; set; } = 0;
        public double totalPurchase { get; set; } = 0;
        public double totalPay { get; set; } = 0;
        public double ToDayTotalPay { get; set; } = 0;
        public IEnumerable<TopSupplierViewModel> Top10Suppliers { get; set; } = new List<TopSupplierViewModel>();
    }
}
