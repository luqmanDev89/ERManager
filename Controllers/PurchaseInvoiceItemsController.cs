using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PurchaseInvoiceItemsController : Controller
    {
        private readonly ERManagerContext _context;

        public PurchaseInvoiceItemsController(ERManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: PurchaseInvoiceItems
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.PurchaseInvoiceItem.Include(p => p.Product).Include(p => p.PurchaseInvoice);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: PurchaseInvoiceItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoiceItem = await _context.PurchaseInvoiceItem
                .Include(p => p.Product)
                .Include(p => p.PurchaseInvoice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseInvoiceItem == null)
            {
                return NotFound();
            }

            return View(purchaseInvoiceItem);
        }

        // GET: PurchaseInvoiceItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["PurchaseInvoiceId"] = new SelectList(_context.PurchaseInvoice, "Id", "Id");
            return View();
        }

        // POST: PurchaseInvoiceItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PurchaseInvoiceId,ProductId,Quantity,UnitPrice")] PurchaseInvoiceItem purchaseInvoiceItem)
        {
            _context.Add(purchaseInvoiceItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseInvoiceItem.ProductId);
            ViewData["PurchaseInvoiceId"] = new SelectList(_context.PurchaseInvoice, "Id", "Id", purchaseInvoiceItem.PurchaseInvoiceId);
            return View(purchaseInvoiceItem);
        }

        // GET: PurchaseInvoiceItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoiceItem = await _context.PurchaseInvoiceItem.FindAsync(id);
            if (purchaseInvoiceItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseInvoiceItem.ProductId);
            ViewData["PurchaseInvoiceId"] = new SelectList(_context.PurchaseInvoice, "Id", "Id", purchaseInvoiceItem.PurchaseInvoiceId);
            return View(purchaseInvoiceItem);
        }

        // POST: PurchaseInvoiceItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PurchaseInvoiceId,ProductId,Quantity,UnitPrice")] PurchaseInvoiceItem purchaseInvoiceItem)
        {
            if (id != purchaseInvoiceItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseInvoiceItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseInvoiceItemExists(purchaseInvoiceItem.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseInvoiceItem.ProductId);
            ViewData["PurchaseInvoiceId"] = new SelectList(_context.PurchaseInvoice, "Id", "Id", purchaseInvoiceItem.PurchaseInvoiceId);
            return View(purchaseInvoiceItem);
        }

        // GET: PurchaseInvoiceItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoiceItem = await _context.PurchaseInvoiceItem
                .Include(p => p.Product)
                .Include(p => p.PurchaseInvoice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseInvoiceItem == null)
            {
                return NotFound();
            }

            return View(purchaseInvoiceItem);
        }

        // POST: PurchaseInvoiceItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseInvoiceItem = await _context.PurchaseInvoiceItem.FindAsync(id);
            if (purchaseInvoiceItem == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "purchaseInvoiceItem not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.PurchaseInvoiceItem.Remove(purchaseInvoiceItem);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "purchaseInvoiceItem deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the purchaseInvoiceItem.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool PurchaseInvoiceItemExists(int id)
        {
            return _context.PurchaseInvoiceItem.Any(e => e.Id == id);
        }
    }
}
