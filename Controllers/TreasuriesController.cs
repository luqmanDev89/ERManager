using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Treasury;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TreasuriesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreasuriesController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // Accessing the session here where HttpContext is available
            string selectedBranchIdStr = _httpContextAccessor.HttpContext?.Session.GetString("SelectedBranch") ?? "1";
            if (selectedBranchIdStr != null)
            {
                _BranchId = int.Parse(selectedBranchIdStr);
            }
            _context = context ?? throw new ArgumentNullException(nameof(context));
            // Set default currency ID based on available currencies in the database
            var defaultCurrency = _context.Currency.FirstOrDefault();
            _DefaultCurrencyId = defaultCurrency?.CurrencyId ?? 1;
        }

        // GET: Treasuries
        public async Task<IActionResult> Index(string? Name)
        {
            var eRManagerContext = _context.Treasuries.Where(d => d.BranchId == _BranchId).Include(t => t.Branch).Include(t => t.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                eRManagerContext = eRManagerContext.Where(d => d.Name.Contains(Name) ||
                d.Notes.Contains(Name)
                );
            }
            ViewBag.Name = Name;
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: Treasuries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasury = await _context.Treasuries.Where(d => d.BranchId == _BranchId)
                .Include(t => t.Branch)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TreasuryId == id);
            if (treasury == null)
            {
                return NotFound();
            }

            return View(treasury);
        }

        // GET: Treasuries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Treasuries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreasuryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve user ID from claims inside the action method
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Load User and Branch
                var user = await _context.Users.FindAsync(userId);
                var branch = await _context.Branches.FindAsync(_BranchId);

                if (user == null)
                {
                    // Handle the case when User or Branch is not found
                    ModelState.AddModelError(string.Empty, "User  not found.");
                    return View(model);
                }
                if (branch == null)
                {
                    // Handle the case when User or Branch is not found
                    ModelState.AddModelError(string.Empty, "Branch  not found.");
                    return View(model);
                }

                // Create a new Treasury instance
                var treasury = new Treasury
                {
                    Name = model.Name,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    UserId = user.Id, // Ensure you have a valid user ID
                    User = user,           // Set the User property
                    BranchId = branch.Id, // Ensure you have a valid branch ID
                    Branch = branch        // Set the Branch property
                };

                // Add the treasury entity to the context
                _context.Add(treasury);
                await _context.SaveChangesAsync();

                // Redirect to the index view after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, return the view with the current model state
            return View(model);
        }




        // GET: Treasuries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasury = await _context.Treasuries.FindAsync(id);
            if (treasury == null)
            {
                return NotFound();
            }

            // Map the Treasury entity to the TreasuryViewModel
            var treasuryViewModel = new TreasuryViewModel
            {
                TreasuryId = treasury.TreasuryId,
                Name = treasury.Name,
                Notes = treasury.Notes,
                CreatedAt = treasury.CreatedAt,

            };

            return View(treasuryViewModel);
        }


        // POST: Treasuries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreasuryId, Name, Notes, CreatedAt")] TreasuryViewModel treasuryViewModel)
        {
            if (id != treasuryViewModel.TreasuryId)
            {
                return NotFound();
            }
            // Retrieve user ID from claims inside the action method
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Load User and Branch
            var user = await _context.Users.FindAsync(userId);
            var branch = await _context.Branches.FindAsync(_BranchId);

            if (user == null || branch == null)
            {
                ModelState.AddModelError(string.Empty, "User or Branch not found.");
                return View(treasuryViewModel);
            }

            var treasury = new Treasury
            {
                TreasuryId = treasuryViewModel.TreasuryId,
                Name = treasuryViewModel.Name,
                Notes = treasuryViewModel.Notes,
                CreatedAt = treasuryViewModel.CreatedAt,
                User = user,
                Branch = branch,
                UpdateAt = DateTime.Now // Assuming you want to update this too
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treasury);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreasuryExists(treasury.TreasuryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(treasuryViewModel);
        }






        // GET: Treasuries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasury = await _context.Treasuries
                .Include(t => t.Branch)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TreasuryId == id);
            if (treasury == null)
            {
                return NotFound();
            }

            return View(treasury);
        }

        // POST: Treasuries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treasury = await _context.Treasuries.FindAsync(id);
            if (treasury == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "treasury not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Treasuries.Remove(treasury);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "treasury deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the treasury.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool TreasuryExists(int id)
        {
            return _context.Treasuries.Any(e => e.TreasuryId == id);
        }

        public ActionResult Dashboard()
        {
            var today = DateTime.Now.Date;

            var Treasuries = _context.Treasuries.Where(d => d.BranchId == _BranchId)
                .Include(c => c.ContactPayments)
                .ThenInclude(c => c.Contact)
                .Include(c => c.MoneyTransactions)
                .Include(c => c.Expenses)
                .Include(c => c.TreasuryMoneyTransfersAsSource)
                .Include(c => c.TreasuryMoneyTransfersAsDestination)
                .Include(c => c.User)
                .ToList();

            // Populate TreasuriesActivities with today's activities
            var TreasuriesActivities = Treasuries.SelectMany(t =>
                t.ContactPayments
                    .Where(cp => cp.CreatedAt.Date == today && cp.PaymentType == PaymentType.Outflow)
                    .Select(cp => new TreasuriesActivityViewModel
                    {
                        TreasuryId = t.TreasuryId,
                        Name = t.Name + " -> " + cp.Contact.Name,  // Corrected this line
                        Notes = cp.Notes,
                        CreatedAt = cp.CreatedAt,
                        UpdateAt = cp.UpdateAt,
                        Amount = cp.Amount,
                        UserName = cp.User.UserName,
                        Type = "پارەدان"
                    })
                .Concat(
                    t.ContactPayments
                    .Where(cp => cp.CreatedAt.Date == today && cp.PaymentType == PaymentType.Inflow)
                    .Select(cp => new TreasuriesActivityViewModel
                    {
                        TreasuryId = t.TreasuryId,
                        Name = t.Name,
                        Notes = cp.Notes,
                        CreatedAt = cp.CreatedAt,
                        UpdateAt = cp.UpdateAt,
                        Amount = cp.Amount,
                        UserName = cp.User.UserName,
                        Type = "پارەوەرگرتن"
                    })
                )
                .Concat(
                    t.MoneyTransactions
                        .Where(mt => mt.CreatedAt.Date == today && mt.TransactionType == TransactionType.Out)
                        .Select(mt => new TreasuriesActivityViewModel
                        {
                            TreasuryId = t.TreasuryId,
                            Name = t.Name,
                            Notes = mt.Notes,
                            CreatedAt = mt.CreatedAt,
                            UpdateAt = mt.UpdateAt,
                            Amount = mt.Amount,
                            UserName = mt.User.UserName,
                            Type = "ڕاکێشانی پارە"
                        })
                )
                .Concat(
                    t.MoneyTransactions
                        .Where(mt => mt.CreatedAt.Date == today && mt.TransactionType == TransactionType.In)
                        .Select(mt => new TreasuriesActivityViewModel
                        {
                            TreasuryId = t.TreasuryId,
                            Name = t.Name,
                            Notes = mt.Notes,
                            CreatedAt = mt.CreatedAt,
                            UpdateAt = mt.UpdateAt,
                            Amount = mt.Amount,
                            UserName = mt.User.UserName,
                            Type = "زیادکردنی پارە"
                        })
                )
                .Concat(
                    t.Expenses
                        .Where(exp => exp.CreatedAt.Date == today)
                        .Select(exp => new TreasuriesActivityViewModel
                        {
                            TreasuryId = t.TreasuryId,
                            Name = t.Name,
                            Notes = exp.Notes,
                            CreatedAt = exp.CreatedAt,
                            UpdateAt = exp.UpdateAt,
                            Amount = exp.Amount,
                            UserName = exp.User.UserName,
                            Type = "مەسروف"
                        })
                )
                .Concat(
                    t.TreasuryMoneyTransfersAsSource
                        .Where(tmt => tmt.CreatedAt.Date == today)
                        .Select(tmt => new TreasuriesActivityViewModel
                        {
                            TreasuryId = t.TreasuryId,
                            Name = t.Name,
                            Notes = tmt.Notes,
                            CreatedAt = tmt.CreatedAt,
                            UpdateAt = tmt.UpdateAt,
                            Amount = tmt.Amount,
                            UserName = tmt.User.UserName,
                            Type = "گواستنەوەی پارە"
                        })
                )
                .Concat(
                    t.TreasuryMoneyTransfersAsDestination
                        .Where(tmt => tmt.CreatedAt.Date == today)
                        .Select(tmt => new TreasuriesActivityViewModel
                        {
                            TreasuryId = t.TreasuryId,
                            Name = t.Name,
                            Notes = tmt.Notes,
                            CreatedAt = tmt.CreatedAt,
                            UpdateAt = tmt.UpdateAt,
                            Amount = tmt.Amount,
                            UserName = tmt.User.UserName,
                            Type = "گوستنەوەی پارە"
                        })
                )
            ).ToList(); // Convert to List<TreasuriesActivityViewModel> after flattening

            var dashboardViewModel = new DashboardViewModel
            {
                TreasuryTotal = Treasuries.Select(t => new TreasuryTotalViewModel
                {
                    TreasuryId = t.TreasuryId,
                    Name = t.Name,
                    InflowMoneyTransactions = t.MoneyTransactions
                        .Where(mt => mt.TransactionType == TransactionType.In)
                        .Sum(mt => (double?)mt.Amount) ?? 0,
                    OutflowMoneyTransactions = t.MoneyTransactions
                        .Where(mt => mt.TransactionType == TransactionType.Out)
                        .Sum(mt => (double?)mt.Amount) ?? 0,
                    InflowContactPayments = t.ContactPayments
                        .Where(cp => cp.PaymentType == PaymentType.Inflow)
                        .Sum(cp => (double?)cp.Amount) ?? 0,
                    OutflowContactPayments = t.ContactPayments
                        .Where(cp => cp.PaymentType == PaymentType.Outflow)
                        .Sum(cp => (double?)cp.Amount) ?? 0,
                    Expenses = t.Expenses.Sum(exp => (double?)exp.Amount) ?? 0,
                    TreasuryTotal =
                     ((t.MoneyTransactions
                        .Where(mt => mt.TransactionType == TransactionType.In)
                        .Sum(mt => (double?)mt.Amount) ?? 0) +
                     (t.ContactPayments
                        .Where(cp => cp.PaymentType == PaymentType.Inflow)
                        .Sum(cp => (double?)cp.Amount) ?? 0) +
                     (t.TreasuryMoneyTransfersAsDestination
                        .Sum(mt => (double?)mt.Amount) ?? 0)) -
                    ((t.MoneyTransactions
                        .Where(mt => mt.TransactionType == TransactionType.Out)
                        .Sum(mt => (double?)mt.Amount) ?? 0) +
                     (t.ContactPayments
                        .Where(cp => cp.PaymentType == PaymentType.Outflow)
                        .Sum(cp => (double?)cp.Amount) ?? 0) +
                     (t.TreasuryMoneyTransfersAsSource
                        .Sum(mt => (double?)mt.Amount) ?? 0)+
                        (t.Expenses
                        .Sum(mt => (double?)mt.Amount) ?? 0))

                }).ToList(),

                TreasuriesActivities = TreasuriesActivities // Set the TreasuriesActivities correctly
            };

            return View(dashboardViewModel);
        }

    }
}
