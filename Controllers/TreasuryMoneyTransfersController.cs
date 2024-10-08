using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERManager.Data;
using ERManager.Models;

namespace ERManager.Controllers
{
    public class TreasuryMoneyTransfersController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _DefaultCurrencyId = 1;
        public TreasuryMoneyTransfersController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: TreasuryMoneyTransfers
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.TreasuryMoneyTransfers.Include(t => t.Currency).Include(t => t.DestinationTreasury).Include(t => t.SourceTreasury).Include(t => t.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: TreasuryMoneyTransfers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers
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
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code");
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password");
            return View();
        }

        // POST: TreasuryMoneyTransfers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransferId,SourceTreasuryId,DestinationTreasuryId,Amount,CreatedAt,Notes")] TreasuryMoneyTransfer treasuryMoneyTransfer)
        {
            if (ModelState.IsValid)
            {
                treasuryMoneyTransfer.UserId = _LogUserId;
                treasuryMoneyTransfer.CurrencyId = _DefaultCurrencyId;
                treasuryMoneyTransfer.UpdateAt = DateTime.Now;
                _context.Add(treasuryMoneyTransfer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code", treasuryMoneyTransfer.CurrencyId);
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", treasuryMoneyTransfer.UserId);
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
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code", treasuryMoneyTransfer.CurrencyId);
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", treasuryMoneyTransfer.UserId);
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
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code", treasuryMoneyTransfer.CurrencyId);
            ViewData["DestinationTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.DestinationTreasuryId);
            ViewData["SourceTreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", treasuryMoneyTransfer.SourceTreasuryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", treasuryMoneyTransfer.UserId);
            return View(treasuryMoneyTransfer);
        }

        // GET: TreasuryMoneyTransfers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treasuryMoneyTransfer = await _context.TreasuryMoneyTransfers
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
            if (treasuryMoneyTransfer != null)
            {
                _context.TreasuryMoneyTransfers.Remove(treasuryMoneyTransfer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreasuryMoneyTransferExists(int id)
        {
            return _context.TreasuryMoneyTransfers.Any(e => e.TransferId == id);
        }
    }
}
