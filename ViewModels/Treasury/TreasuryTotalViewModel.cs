namespace ERManager.ViewModels.Treasury
{
    public class TreasuryTotalViewModel
    {
        public int TreasuryId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public double InflowMoneyTransactions { get; set; } = 0;
        public double OutflowMoneyTransactions { get; set; } = 0;
        public double InflowContactPayments { get; set; } = 0;
        public double OutflowContactPayments { get; set; } = 0;
        public double Expenses { get; set; } = 0;
        public double TreasuryTotal { get; set; } = 0;
    }
}
