namespace ERManager.ViewModels.Treasury
{
    public class DashboardViewModel
    {
        // List to store the total information of treasuries
        public List<TreasuryTotalViewModel> TreasuryTotal { get; set; } = new List<TreasuryTotalViewModel>();

        // List to store the activity logs for treasuries
        public List<TreasuriesActivityViewModel> TreasuriesActivities { get; set; } = new List<TreasuriesActivityViewModel>();
    }
}
