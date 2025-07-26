namespace ERManager.ViewModels.Contacts
{
    public class BalanceViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int InvoiceId { get; set; }
        public string Type { get; set; } // "SaleInvoice" or "ContactPayment"
        public double TotalAmount { get; set; } // Amount for each transaction
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Balance { get; set; } = 0;
        public double debtor { get; set; } = 0;
        public double creditor { get; set; } = 0;
    }
}
