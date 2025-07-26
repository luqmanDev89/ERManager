namespace ERManager.ViewModels.Homes
{
    public class HomePageViewModel
    {
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string BranchName { get; set; }

        // Financial metrics
        public double TotalPurchase { get; set; }
        public double TotalSales { get; set; }
        public double ContactPaymentInflow { get; set; }
        public double ContactPaymentOutflow { get; set; }
        public double MoneyTransactionIn { get; set; }
        public double MoneyTransactionOut { get; set; }
        public double TotalExpenses { get; set; }

        // Placeholder for RecentActivities and BestCustomer
        public List<Activity> RecentActivities { get; set; } // Change to List<Activity> for proper handling
        public List<Customer> BestCustomer { get; set; } // Change to List<Customer> for proper handling
        public List<MonthlyTotal> MonthlyTotals { get; set; }
    }

    public class Activity
    {
        public DateTime CreatedAt { get; set; } // Assuming you want to use CreatedAt for recent activities
        public string ActivityName { get; set; }
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContacPhone { get; set; }
        public string Status { get; set; } = "Completed";
        public double Amount { get; set; }
    }

    public class Customer
    {
        public string CustomerName { get; set; } // Change property to match the usage in the Razor view
        public double TotalSales { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }
    public class MonthlyTotal
    {
        public string Month { get; set; }
        public double TotalAmount { get; set; }
    }
}
