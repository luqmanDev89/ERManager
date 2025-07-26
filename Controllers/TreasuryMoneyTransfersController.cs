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
    public class TreasuryMoneyTransfersController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TreasuryMoneyTransfersController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: TreasuryMoneyTransfers
        public async Task<IActionResult> Index(int? SourceTreasuryId, int? DestinationTreasuryId, DateTime? startDate, DateTime? endDate, string? Notes)
        {
            var eRManagerContext = _context.TreasuryMoneyTransfers.Where(d => d.SourceTreasury.BranchId == _BranchId && d.DestinationTreasury.BranchId == _BranchId).Include(t => t.Currency).Include(t => t.DestinationTreasury).Include(t => t.SourceTreasury).Include(t => t.User).AsQueryable();
            if (SourceTreasuryId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.SourceTreasuryId == SourceTreasuryId);
            }
            if (DestinationTreasuryId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.DestinationTreasuryId == DestinationTreasuryId);
            }

            if (!string.IsNullOrEmpty(Notes))
            {
                eRManagerContext = eRManagerContext.Where(e => e.Notes != null && e.Notes.Contains(Notes));
            }

            // Pass categories to populate the Category filter dropdown
            ViewBag.Notes = Notes;
            ViewBag.SourceTreasuryId = SourceTreasuryId;
            ViewBag.DestinationTreasuryId = DestinationTreasuryId;
            ViewBag.startDate = startDate ?? DateTime.Now;
            ViewBag.endDate = endDate ?? DateTime.Now;
            ViewBag.Treasuries = _context.Treasuries.ToList();
            ViewBag.total = eRManagerContext.Sum(d => (double)d.Amount);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: TreasuryMoneyTransfers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers.Where(d => d.SourceTreasury.BranchId == _BranchId && d.DestinationTreasury.BranchId == _BranchId)
                .Include(t => t.Currency)
                .Include(t => t.DestinationTreasury)
                .Include(t => t.SourceTreasury)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TransferId == id);
            if (treasuryMoneyTransfer == null)
            {
                return NotFound();
            }

            return View(treasuryMoneyTransfer);
        }

        // GET: TreasuryMoneyTransfers/Create
        public IActionResult Create()
        {
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name");
            return View();
        }

        // POST: TreasuryMoneyTransfers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransferId,SourceTreasuryId,DestinationTreasuryId,Amount,CreatedAt,Notes")] TreasuryMoneyTransfer treasuryMoneyTransfer)
        {
            // Manual validation
            bool isValid = true;

            if (treasuryMoneyTransfer.SourceTreasuryId == 0)
            {
                ModelState.AddModelError("SourceTreasuryId", "Source Treasury is required.");
                isValid = false;
            }

            if (treasuryMoneyTransfer.DestinationTreasuryId == 0)
            {
                ModelState.AddModelError("DestinationTreasuryId", "Destination Treasury is required.");
                isValid = false;
            }

            if (treasuryMoneyTransfer.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be greater than zero.");
                isValid = false;
            }

            if (string.IsNullOrEmpty(treasuryMoneyTransfer.Notes))
            {
                ModelState.AddModelError("Notes", "Notes are required.");
                isValid = false;
            }

            if (isValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                treasuryMoneyTransfer.UserId = userId;
                treasuryMoneyTransfer.CurrencyId = _DefaultCurrencyId;
                treasuryMoneyTransfer.UpdateAt = DateTime.Now;
                _context.Add(treasuryMoneyTransfer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);

            return View(treasuryMoneyTransfer);
        }


        // GET: TreasuryMoneyTransfers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers.FindAsync(id);
            if (treasuryMoneyTransfer == null)
            {
                return NotFound();
            }
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);
            return View(treasuryMoneyTransfer);
        }

        // POST: TreasuryMoneyTransfers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransferId,SourceTreasuryId,DestinationTreasuryId,Amount,CreatedAt,Notes")] TreasuryMoneyTransfer treasuryMoneyTransfer)
        {
            if (id != treasuryMoneyTransfer.TransferId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    treasuryMoneyTransfer.UserId = userId;
                    treasuryMoneyTransfer.CurrencyId = _DefaultCurrencyId;
                    treasuryMoneyTransfer.UpdateAt = DateTime.Now;
                    _context.Update(treasuryMoneyTransfer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreasuryMoneyTransferExists(treasuryMoneyTransfer.TransferId))
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
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries.Where(d => d.BranchId == _BranchId), "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);
            return View(treasuryMoneyTransfer);
        }

        // GET: TreasuryMoneyTransfers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers.Where(d => d.SourceTreasury.BranchId == _BranchId && d.DestinationTreasury.BranchId == _BranchId)
                .Include(t => t.Currency)
                .Include(t => t.DestinationTreasury)
                .Include(t => t.SourceTreasury)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TransferId == id);
            if (treasuryMoneyTransfer == null)
            {
                return NotFound();
            }

            return View(treasuryMoneyTransfer);
        }

        // POST: TreasuryMoneyTransfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers.FindAsync(id);
            if (treasuryMoneyTransfer == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "treasuryMoneyTransfer not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.TreasuryMoneyTransfers.Remove(treasuryMoneyTransfer);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "treasuryMoneyTransfer deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the treasuryMoneyTransfer.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool TreasuryMoneyTransferExists(int id)
        {
            return _context.TreasuryMoneyTransfers.Any(e => e.TransferId == id);
        }
    }
}
