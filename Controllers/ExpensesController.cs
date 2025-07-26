using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Expenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ExpensesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExpensesController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IActionResult> Dashboard()
        {
            var totalExpenses = await _context.Expenses.Where(d => d.BranchId == _BranchId)
                .SumAsync(e => (double)e.Amount);

            var expensesByCategory = await _context.Expenses.Where(d => d.BranchId == _BranchId)
                .GroupBy(e => e.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => (double)e.Amount) // Cast to double
                })
                .ToListAsync();

            var recentExpenses = await _context.Expenses.Where(d => d.BranchId == _BranchId)
                .Include(e => e.Category)
                .Include(e => e.Branch)
                .Include(e => e.User)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.ExpensesByCategory = expensesByCategory;
            ViewBag.RecentExpenses = recentExpenses;

            return View();
        }

        public async Task<IActionResult> Index(int? CategoryId, int? TreasuryId, DateTime? startDate, DateTime? endDate, string? Notes)
        {
            var expenses = _context.Expenses.Where(d => d.BranchId == _BranchId)
                .Include(e => e.Category)
                .Include(e => e.Treasury)
                .Include(e => e.Branch)
                .Include(e => e.User)
                .AsQueryable();

            if (CategoryId != null)
            {
                expenses = expenses.Where(e => e.CategoryId == CategoryId);
            }
            if (TreasuryId != null)
            {
                expenses = expenses.Where(e => e.TreasuryId == TreasuryId);
            }
            if (!string.IsNullOrEmpty(Notes))
            {
                expenses = expenses.Where(e => e.Notes != null && e.Notes.Contains(Notes));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                expenses = expenses.Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);
            }
            else
            {
                expenses = expenses.Where(e => e.CreatedAt >= DateTime.Now.Date && e.CreatedAt <= DateTime.Now.Date);
            }

            var totalAmount = expenses.Select(e => (double)e.Amount).Sum();

            ViewBag.TotalAmount = totalAmount;
            ViewBag.TreasuryId = TreasuryId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.Notes = Notes;
            ViewBag.startDate = startDate ?? DateTime.Now;
            ViewBag.endDate = endDate ?? DateTime.Now;

            ViewBag.Categories = await _context.ExpensesCategory.ToListAsync();
            ViewBag.Treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

            return View(expenses.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.Where(d => d.BranchId == _BranchId)
                .Include(e => e.Branch)
                .Include(e => e.Category)
                .Include(e => e.Currency)
                .Include(e => e.Treasury)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenses == null)
            {
                return NotFound();
            }

            return View(expenses);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _context.ExpensesCategory.ToListAsync();
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

            if (!categories.Any())
            {

                return RedirectToAction("Create", "ExpensesCategories");
            }
            if (!treasuries.Any())
            {
                return RedirectToAction("Create", "Treasuries");
            }

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(treasuries, "TreasuryId", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpensesViewModel expensesViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var expense = new Expenses
                    {
                        Notes = expensesViewModel.Notes,
                        Amount = expensesViewModel.Amount,
                        TreasuryId = expensesViewModel.TreasuryId,
                        CategoryId = expensesViewModel.CategoryId,
                        UserId = userId, // Use UserId from claims
                        BranchId = _BranchId, // Retrieve BranchId from session
                        CurrencyId = _DefaultCurrencyId,
                        CreatedAt = expensesViewModel.CreatedAt,
                        UpdateAt = DateTime.Now
                        
                    };

                    _context.Add(expense);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while creating the expense: {ex.Message}");
                }
            }

            ViewBag.TreasuryId = new SelectList(await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync(), "TreasuryId", "Name");
            ViewBag.CategoryId = new SelectList(await _context.ExpensesCategory.ToListAsync(), "Id", "Name");

            return View(expensesViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.ExpensesCategory, "Id", "Name", expenses.CategoryId);
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", expenses.TreasuryId);
            return View(expenses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Notes,Amount,CreatedAt,TreasuryId,CategoryId")] Expenses expenses)
        {
            if (id != expenses.Id)
            {
                return NotFound();
            }

            // Manual validation checks
            if (expenses.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero.");
            }

            if (!_context.ExpensesCategory.Any(c => c.Id == expenses.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Selected category is invalid.");
            }

            if (!_context.Treasuries.Any(t => t.TreasuryId == expenses.TreasuryId && t.BranchId == _BranchId))
            {
                ModelState.AddModelError("TreasuryId", "Selected treasury is invalid for the current branch.");
            }

            try
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                expenses.BranchId = _BranchId; // Retrieve BranchId from session
                expenses.CurrencyId = _DefaultCurrencyId;
                expenses.UpdateAt = DateTime.Now;
                expenses.UserId = userId; // Use UserId from claims

                _context.Update(expenses);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(expenses);
            }
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses.Where(d => d.BranchId == _BranchId)
                .Include(e => e.Branch)
                .Include(e => e.Category)
                .Include(e => e.Treasury)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenses == null)
            {
                return NotFound();
            }

            return View(expenses);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "expenses not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Expenses.Remove(expenses);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "expenses deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the expenses.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool ExpensesExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
