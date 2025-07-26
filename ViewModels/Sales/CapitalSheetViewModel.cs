namespace ERManager.ViewModels.Sales
{
    public class CapitalSheetViewModel
    {
        public double TotalSellerDebt { get; set; }
        public double TotalBuyerDebt { get; set; }
        public double TotalTreasuriesBalance { get; set; }

        // Computed property for FinalTotal
        public double FinalTotal => (TotalBuyerDebt + TotalTreasuriesBalance) - TotalSellerDebt;
    }
}
