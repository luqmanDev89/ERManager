using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class SaleInvoiceItemsController : Controller
    {
        private readonly ERManagerContext _context;

        public SaleInvoiceItemsController(ERManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: SaleInvoiceItems
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.SaleInvoiceItems.Include(s => s.Product).Include(s => s.SaleInvoice);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: SaleInvoiceItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleInvoiceItem = await _context.SaleInvoiceItems
                .Include(s => s.Product)
                .Include(s => s.SaleInvoice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleInvoiceItem == null)
            {
                return NotFound();
            }

            return View(saleInvoiceItem);
        }

        // GET: SaleInvoiceItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["SaleInvoiceId"] = new SelectList(_context.SaleInvoices, "Id", "Id");
            return View();
        }

        // POST: SaleInvoiceItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SaleInvoiceId,ProductId,Quantity,UnitPrice")] SaleInvoiceItem saleInvoiceItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saleInvoiceItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleInvoiceItem.ProductId);
            ViewData["SaleInvoiceId"] = new SelectList(_context.SaleInvoices, "Id", "Id", saleInvoiceItem.SaleInvoiceId);
            return View(saleInvoiceItem);
        }

        // GET: SaleInvoiceItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleInvoiceItem = await _context.SaleInvoiceItems.FindAsync(id);
            if (saleInvoiceItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleInvoiceItem.ProductId);
            ViewData["SaleInvoiceId"] = new SelectList(_context.SaleInvoices, "Id", "Id", saleInvoiceItem.SaleInvoiceId);
            return View(saleInvoiceItem);
        }

        // POST: SaleInvoiceItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SaleInvoiceId,ProductId,Quantity,UnitPrice")] SaleInvoiceItem saleInvoiceItem)
        {
            if (id != saleInvoiceItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(saleInvoiceItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleInvoiceItemExists(saleInvoiceItem.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", saleInvoiceItem.ProductId);
            ViewData["SaleInvoiceId"] = new SelectList(_context.SaleInvoices, "Id", "Id", saleInvoiceItem.SaleInvoiceId);
            return View(saleInvoiceItem);
        }

        // GET: SaleInvoiceItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleInvoiceItem = await _context.SaleInvoiceItems
                .Include(s => s.Product)
                .Include(s => s.SaleInvoice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleInvoiceItem == null)
            {
                return NotFound();
            }

            return View(saleInvoiceItem);
        }

        // POST: SaleInvoiceItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saleInvoiceItem = await _context.SaleInvoiceItems.FindAsync(id);
            if (saleInvoiceItem != null)
            {
                _context.SaleInvoiceItems.Remove(saleInvoiceItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleInvoiceItemExists(int id)
        {
            return _context.SaleInvoiceItems.Any(e => e.Id == id);
        }
    }
}
