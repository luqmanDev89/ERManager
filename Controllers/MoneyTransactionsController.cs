using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class MoneyTransactionsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MoneyTransactionsController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;                                                   // Accessing the session here where HttpContext is available
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

        // GET: MoneyTransactions
        public async Task<IActionResult> Index(string? TransactionType, int? TreasuryId, DateTime? startDate, DateTime? endDate, string? Notes)
        {
            var eRManagerContext = _context.MoneyTransactions.Where(d => d.Treasury.BranchId == _BranchId).Include(m => m.Currency).Include(m => m.Treasury).Include(m => m.User).AsQueryable();

            if (TreasuryId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.TreasuryId == TreasuryId);
            }

            if (!string.IsNullOrEmpty(Notes))
            {
                eRManagerContext = eRManagerContext.Where(e => e.Notes != null && e.Notes.Contains(Notes));
            }
            if (!string.IsNullOrEmpty(TransactionType))
            {
                // Try to parse the string to the PaymentType enum
                if (Enum.TryParse(TransactionType, out TransactionType parsedTransactionType))
                {
                    eRManagerContext = eRManagerContext.Where(e => e.TransactionType == parsedTransactionType);
                }
            }

            // Pass categories to populate the Category filter dropdown
            ViewBag.Notes = Notes;
            ViewBag.TreasuryId = TreasuryId;
            ViewBag.startDate = startDate ?? DateTime.Now;
            ViewBag.endDate = endDate ?? DateTime.Now;
            ViewBag.Contacts = _context.Contact.ToList();
            ViewBag.Treasuries = _context.Treasuries.ToList();
            ViewBag.totalIn = eRManagerContext.Where(d => d.TransactionType == Models.TransactionType.In).ToList().Sum(d => (double)d.Amount);
            ViewBag.totalOut = eRManagerContext.Where(d => d.TransactionType == Models.TransactionType.Out).ToList().Sum(d => (double)d.Amount);
            ViewBag.total = eRManagerContext.Where(d => d.TransactionType == Models.TransactionType.In).ToList().Sum(d => (double)d.Amount) - eRManagerContext.Where(d => d.TransactionType == Models.TransactionType.Out).ToList().Sum(d => (double)d.Amount);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: MoneyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moneyTransaction = await _context.MoneyTransactions.Where(d => d.Treasury.BranchId == _BranchId)
                .Include(m => m.Currency)
                .Include(m => m.Treasury)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (moneyTransaction == null)
            {
                return NotFound();
            }

            return View(moneyTransaction);
        }

        // GET: MoneyTransactions/Create
        public IActionResult Create()
        {
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View();
        }
        // GET: MoneyTransactions/Create
        public IActionResult CreateOut()
        {
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View();
        }

        // POST: MoneyTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,Amount,TransactionType,CreatedAt,TreasuryId,Notes")] MoneyTransaction moneyTransaction)
        {
            if (!string.IsNullOrWhiteSpace(moneyTransaction.Amount.ToString()) ||
                !string.IsNullOrWhiteSpace(moneyTransaction.TransactionType.ToString()) ||
                !string.IsNullOrWhiteSpace(moneyTransaction.CreatedAt.ToString()) ||
                moneyTransaction.TreasuryId > 0)
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                moneyTransaction.CurrencyId = _DefaultCurrencyId;
                moneyTransaction.UserId = userId;
                moneyTransaction.UpdateAt = DateTime.Now;

                _context.Add(moneyTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View(moneyTransaction);
        }

        // GET: MoneyTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moneyTransaction = await _context.MoneyTransactions.FindAsync(id);
            if (moneyTransaction == null)
            {
                return NotFound();
            }
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View(moneyTransaction);
        }

        // POST: MoneyTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,Amount,TransactionType,CreatedAt,TreasuryId,Notes")] MoneyTransaction moneyTransaction)
        {
            if (id != moneyTransaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    moneyTransaction.UpdateAt = DateTime.Now;
                    moneyTransaction.UserId = userId;
                    moneyTransaction.CurrencyId = _DefaultCurrencyId;
                    _context.Update(moneyTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoneyTransactionExists(moneyTransaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View(moneyTransaction);
        }

        // GET: MoneyTransactions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moneyTransaction = await _context.MoneyTransactions
                .Include(m => m.Currency)
                .Include(m => m.Treasury)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (moneyTransaction == null)
            {
                return NotFound();
            }

            return View(moneyTransaction);
        }

        // POST: MoneyTransactions/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moneyTransaction = await _context.MoneyTransactions.FindAsync(id);
            if (moneyTransaction == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "moneyTransaction not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.MoneyTransactions.Remove(moneyTransaction);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "moneyTransaction deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the moneyTransaction.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool MoneyTransactionExists(int id)
        {
            return _context.MoneyTransactions.Any(e => e.TransactionId == id);
        }
    }
}
