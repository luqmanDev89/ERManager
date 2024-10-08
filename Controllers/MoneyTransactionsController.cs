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
    public class MoneyTransactionsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _DefaultCurrencyId = 1;

        public MoneyTransactionsController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: MoneyTransactions
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.MoneyTransactions.Include(m => m.Currency).Include(m => m.Treasury).Include(m => m.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: MoneyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: MoneyTransactions/Create
        public IActionResult Create()
        {
            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.TransactionTypes = new SelectList(
                Enum.GetValues(typeof(TransactionType))
                    .Cast<TransactionType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
            return View();
        }

        // POST: MoneyTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,Amount,TransactionType,CreatedAt,TreasuryId,Notes")] MoneyTransaction moneyTransaction)
        {
            if (ModelState.IsValid)
            {
                moneyTransaction.CurrencyId = _DefaultCurrencyId;
                moneyTransaction.UserId = _LogUserId;
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
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
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
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
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
                    moneyTransaction.UpdateAt = DateTime.Now;
                    moneyTransaction.UserId = _LogUserId;
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
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name");
            return View(moneyTransaction);
        }

        // GET: MoneyTransactions/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moneyTransaction = await _context.MoneyTransactions.FindAsync(id);
            if (moneyTransaction != null)
            {
                _context.MoneyTransactions.Remove(moneyTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MoneyTransactionExists(int id)
        {
            return _context.MoneyTransactions.Any(e => e.TransactionId == id);
        }
    }
}
