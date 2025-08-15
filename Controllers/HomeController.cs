using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Homes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _BranchId;
        private readonly ERManagerContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(ERManagerContext context, ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IConfiguration configuration)
            : base(userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _BranchId = int.TryParse(_httpContextAccessor.HttpContext?.Session.GetString("SelectedBranch"), out var branchId) ? branchId : 1;
            _logger = logger;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return RedirectToAction("Login", "Account");

            var userRole = await GetUserRoleAsync(user);
            var branch = await _context.Branches
                .Where(b => b.Id == _BranchId)
                .Select(b => new { b.Id, b.Name })
                .FirstOrDefaultAsync();

            var today = DateTime.Today;

            // Fetch all needed data in a single batch
            var purchaseData = _context.PurchaseInvoice
                .Where(pi => pi.CreatedAt.HasValue && pi.CreatedAt.Value.Date == today && pi.BranchId == _BranchId)
                .Select(pi => new ViewModels.Homes.Activity
                {
                    CreatedAt = pi.CreatedAt ?? DateTime.Now,
                    ActivityName = "کڕین",
                    ContactName = pi.Contact.Name,
                    ContactId = pi.Contact.Id,
                    ContacPhone = pi.Contact.Phone,
                    Amount = pi.InvoiceTotal
                });

            var salesData = _context.SaleInvoices
                .Where(si => si.CreatedAt.HasValue && si.CreatedAt.Value.Date == today && si.BranchId == _BranchId)
                .Select(si => new ViewModels.Homes.Activity
                {
                    CreatedAt = si.CreatedAt ?? DateTime.Now,
                    ActivityName = "فڕۆشتن",
                    ContactName = si.Contact.Name,
                    ContactId = si.Contact.Id,
                    ContacPhone = si.Contact.Phone,
                    Amount = si.InvoiceTotal
                });

            var contactPayments = _context.ContactPayments
                .Where(cp => cp.CreatedAt.Date == today && cp.Contact.BranchId == _BranchId)
                .Select(cp => new ViewModels.Homes.Activity
                {
                    CreatedAt = cp.CreatedAt,
                    ActivityName = cp.PaymentType == PaymentType.Inflow ? "پارەوەرگرتن" : "پارەدان",
                    ContactName = cp.Contact.Name,
                    ContactId = cp.Contact.Id,
                    ContacPhone = cp.Contact.Phone,
                    Amount = cp.Amount
                });

            var moneyTransactions = _context.MoneyTransactions
                .Where(mt => mt.CreatedAt.Date == today && mt.Treasury.BranchId == _BranchId)
                .Select(mt => new ViewModels.Homes.Activity
                {
                    CreatedAt = mt.CreatedAt,
                    ActivityName = mt.TransactionType == TransactionType.In ? "زیادکردن ڕەسید" : "ڕاکێشانی ڕەسید",
                    ContactName = mt.Treasury.Name,
                    ContactId = 0,
                    ContacPhone = "",
                    Amount = mt.Amount
                });

            var expensesData = _context.Expenses
                .Where(e => e.CreatedAt.Date == today && e.BranchId == _BranchId)
                .Select(e => new ViewModels.Homes.Activity
                {
                    CreatedAt = e.CreatedAt,
                    ActivityName = "مەسروف",
                    ContactName = e.Category.Name,
                    ContactId = 0,
                    ContacPhone = "",
                    Amount = e.Amount
                });

            // Fetch best customers in parallel
            var bestCustomersTask = GetBestCustomer();

            // Execute all queries in parallel
            var activities = await Task.WhenAll(
                purchaseData.ToListAsync(),
                salesData.ToListAsync(),
                contactPayments.ToListAsync(),
                moneyTransactions.ToListAsync(),
                expensesData.ToListAsync()
            );

            var bestCustomers = await bestCustomersTask;

            // Populate ViewModel
            var model = new HomePageViewModel
            {
                Title = "Welcome to ER Manager",
                Description = $"Hello, {user.UserName}!",
                Email = user.Email,
                UserImage = "",
                UserName = user.UserName,
                UserRole = userRole,
                BranchName = branch?.Name ?? "Branch Name",
                TotalPurchase = activities[0].Sum(d => d.Amount),
                TotalSales = activities[1].Sum(d => d.Amount),
                ContactPaymentInflow = activities[2].Where(a => a.ActivityName == "پارەوەرگرتن").Sum(d => d.Amount),
                ContactPaymentOutflow = activities[2].Where(a => a.ActivityName == "پارەدان").Sum(d => d.Amount),
                MoneyTransactionIn = activities[3].Where(a => a.ActivityName == "زیادکردن ڕەسید").Sum(d => d.Amount),
                MoneyTransactionOut = activities[3].Where(a => a.ActivityName == "ڕاکێشانی ڕەسید").Sum(d => d.Amount),
                TotalExpenses = activities[4].Sum(d => d.Amount),
                RecentActivities = activities.SelectMany(a => a).Take(50).ToList(), // Pagination: Show only 50 activities
                BestCustomer = bestCustomers,
                MonthlyTotals = await GetMonthlyTotals(DateTime.UtcNow.Year) // Fix: Await the task to get the result
            };

            ViewBag.BranchName = _BranchId;


            return View(model);
        }

        private async Task<List<Customer>> GetBestCustomer()
        {
            return await _context.SaleInvoices
                .Where(si => si.BranchId == _BranchId)
                .GroupBy(si => si.ContactId)
                .Select(g => new Customer
                {
                    CustomerName = g.Select(si => si.Contact.Name).FirstOrDefault(),
                    TotalSales = g.Sum(si => si.SaleInvoiceItems.Sum(item => (item.Quantity * item.UnitPrice) * (1 + (item.TaxPresent / 100)))),
                    LastPurchaseDate = g.Max(si => si.CreatedAt) ?? DateTime.Now
                })
                .OrderByDescending(c => c.TotalSales)
                .Take(5)
                .ToListAsync();
        }
        public async Task<List<MonthlyTotal>> GetMonthlyTotals(int currentYear)
        {
            var monthlyTotals = await _context.SaleInvoices
                .Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Year == currentYear && s.BranchId == _BranchId)
                .Include(d => d.SaleInvoiceItems)
                .AsNoTracking()
                .ToListAsync(); // Fetch all invoices with their items

            // Now group and calculate totals in memory
            var groupedData = monthlyTotals
                .GroupBy(s => s.CreatedAt.Value.Month)
                .Select(g => new MonthlyTotal
                {
                    Month = new DateTime(currentYear, g.Key, 1).ToString("MMMM"),
                    TotalAmount = g.Sum(s => s.SaleInvoiceItems.Sum(item => item.Quantity * item.UnitPrice)) // Sum the items' total for each invoice
                })
                .OrderBy(e => DateTime.ParseExact(e.Month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month) // Order months by name
                .ToList();

            return groupedData;
        }

        [HttpGet]
        public async Task<IActionResult> BackupDatabase1()
        {
            try
            {
                if (_configuration == null)
                {
                    return BadRequest("Configuration is not set.");
                }

                // Get paths from appsettings.json
                string dbPath = _configuration.GetConnectionString("ERManagerContext")?.Replace("Data Source=", "").Trim();
                string sqlitePath = _configuration["DatabaseSettings:SqlitePath"];
                string backupFolder = _configuration["DatabaseSettings:BackupFolder"];

                if (string.IsNullOrEmpty(dbPath) || string.IsNullOrEmpty(sqlitePath) || string.IsNullOrEmpty(backupFolder))
                {
                    return BadRequest("Database or SQLite paths not configured.");
                }

                if (!System.IO.File.Exists(dbPath))
                {
                    return BadRequest("Database file not found.");
                }

                // Ensure backup folder exists
                string backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), backupFolder);
                Directory.CreateDirectory(backupFolderPath); // No-op if exists

                // Generate backup filename
                string backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.sql";
                string backupFilePath = Path.Combine(backupFolderPath, backupFileName);

                // Verify SQLite CLI tool exists
                if (!System.IO.File.Exists(sqlitePath))
                {
                    return BadRequest("SQLite3 executable not found.");
                }

                // Configure process to capture output directly
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = sqlitePath,
                    Arguments = $"\"{dbPath}\" \".dump\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processInfo })
                {
                    process.Start();

                    // Read output and error streams
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
                    {
                        return BadRequest($"Backup failed: {error}");
                    }

                    // Write the SQL dump to file
                    await System.IO.File.WriteAllTextAsync(backupFilePath, output);
                }

                // Return the backup file
                return PhysicalFile(backupFilePath, "application/sql", backupFileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                // Get database path and backup folder from configuration
                string dbPath = _configuration.GetConnectionString("ERManagerContext")?.Replace("Data Source=", "").Trim();
                string backupFolder = _configuration["DatabaseSettings:BackupFolder"];

                if (string.IsNullOrEmpty(dbPath) || string.IsNullOrEmpty(backupFolder))
                    return BadRequest("Database or backup folder not configured.");

                if (!System.IO.File.Exists(dbPath))
                    return BadRequest("Database file not found.");

                // Use absolute path on Linux or relative path within app
                string backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), backupFolder);
                Directory.CreateDirectory(backupFolderPath); // creates folder if it doesn't exist

                // Generate backup filename
                string backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.db";
                string backupFilePath = Path.Combine(backupFolderPath, backupFileName);

                // Perform SQLite backup
                using (var source = new SqliteConnection($"Data Source={dbPath}"))
                using (var destination = new SqliteConnection($"Data Source={backupFilePath}"))
                {
                    await source.OpenAsync();
                    await destination.OpenAsync();
                    source.BackupDatabase(destination);
                }

                // Return the backup file to the client
                return PhysicalFile(backupFilePath, "application/octet-stream", backupFileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RestoreDatabase(IFormFile backupFile)
        {
            try
            {
                // Validate the uploaded file
                if (backupFile == null || backupFile.Length == 0)
                {
                    return Json(new { success = false, message = "No backup file uploaded." });
                }

                // Ensure the file is a valid SQL backup
                if (Path.GetExtension(backupFile.FileName).ToLower() != ".sql")
                {
                    return Json(new { success = false, message = "Invalid file type. Please upload a .sql backup file." });
                }

                // Define the database path
                string dbPath = @"D:\luqman\WebApps\ERManager\ERManagerContext.db";

                if (string.IsNullOrEmpty(dbPath))
                {
                    return Json(new { success = false, message = "Database path not configured." });
                }

                // Verify SQLite CLI tool exists
                string sqlitePath = @"C:\sqlite3\sqlite3.exe";
                if (!System.IO.File.Exists(sqlitePath))
                {
                    return Json(new { success = false, message = "SQLite3 executable not found." });
                }

                // Create a temporary file to store the uploaded backup
                string tempBackupPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempBackupPath, FileMode.Create))
                {
                    await backupFile.CopyToAsync(stream);
                }

                // Prepare the restore command
                string command = $"\"{dbPath}\" < \"{tempBackupPath}\"";

                // Configure process to execute the restore
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = sqlitePath,
                    Arguments = $"\"{dbPath}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processInfo })
                {
                    process.Start();

                    // Write the backup SQL to the process's standard input
                    using (var inputWriter = process.StandardInput)
                    {
                        string backupContent = await System.IO.File.ReadAllTextAsync(tempBackupPath);
                        await inputWriter.WriteAsync(backupContent);
                    }

                    // Read output and error streams
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
                    {
                        return Json(new { success = false, message = $"Restore failed: {error}" });
                    }
                }

                // Clean up the temporary backup file
                System.IO.File.Delete(tempBackupPath);

                return Json(new { success = true, message = "Database restored successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RestorePage()
        {
            return View();
        }

    }
}
