using ERManager.Models;
using ERManager.ViewModels.Purchases;

namespace ERManager.ViewModels.Sales
{
    public class SaleDashboardViewModel
    {

        public IEnumerable<SaleInvoice> Last20Sales { get; set; } = new List<SaleInvoice>();
        public IEnumerable<ContactPayment> Last20ContactPayments { get; set; } = new List<ContactPayment>();
        public double TotalCustomerDebit { get; set; } = 0;
        public double ToDayTotalPurchase { get; set; } = 0;
        public double totalPurchase { get; set; } = 0;
        public double totalPay { get; set; } = 0;
        public double ToDayTotalPay { get; set; } = 0;
        public IEnumerable<TopSupplierViewModel> Top10Suppliers { get; set; } = new List<TopSupplierViewModel>();
    }
}
