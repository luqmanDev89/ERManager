using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Expenses;

namespace ERManager.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId = 1;

        public ExpensesController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.Expenses.Include(e => e.Branch).Include(e => e.Category).Include(e => e.Currency).Include(e => e.Treasury).Include(e => e.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses
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
            var treasuries = await _context.Treasuries.ToListAsync();

            if (!categories.Any() || !treasuries.Any())
            {
                ModelState.AddModelError("", "No categories or treasuries available. Please add them first.");
                return RedirectToAction(nameof(Index)); // or return View with an error message
            }

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            ViewData["TreasuryId"] = new SelectList(treasuries, "TreasuryId", "Name");

            return View();
        }


        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Expenses/Create
        public async Task<IActionResult> Create(ExpensesViewModel expensesViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    

                    var expense = new Expenses
                    {
                        Description = expensesViewModel.Description,
                        Amount = expensesViewModel.Amount,
                        TreasuryId = expensesViewModel.TreasuryId,
                        CategoryId = expensesViewModel.CategoryId,
                        UserId = _LogUserId,
                        BranchId = _BranchId,
                        CurrencyId =_DefaultCurrencyId,
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

            // Repopulate ViewBag items if model state is invalid
            ViewBag.TreasuryId = new SelectList(await _context.Treasuries.ToListAsync(), "TreasuryId", "Name");
            ViewBag.CategoryId = new SelectList(await _context.ExpensesCategory.ToListAsync(), "Id", "Name");

            return View(expensesViewModel);
        }


        // GET: Expenses/Edit/5
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
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", expenses.BranchId);
            ViewData["CategoryId"] = new SelectList(_context.ExpensesCategory, "Id", "Name", expenses.CategoryId);
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code", expenses.CurrencyId);
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", expenses.TreasuryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", expenses.UserId);
            return View(expenses);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Amount,CreatedAt,TreasuryId,CategoryId")] Expenses expenses)
        {
            if (id != expenses.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    expenses.BranchId = _BranchId;
                    expenses.CurrencyId = _DefaultCurrencyId;
                    expenses.UpdateAt = DateTime.Now;
                    expenses.UserId = _LogUserId;

                    _context.Update(expenses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpensesExists(expenses.Id))
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
            ViewData["BranchId"] = new SelectList(_context.Branches, "Id", "Name", expenses.BranchId);
            ViewData["CategoryId"] = new SelectList(_context.ExpensesCategory, "Id", "Name", expenses.CategoryId);
            ViewData["CurrencyId"] = new SelectList(_context.Currency, "CurrencyId", "Code", expenses.CurrencyId);
            ViewData["TreasuryId"] = new SelectList(_context.Treasuries, "TreasuryId", "Name", expenses.TreasuryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", expenses.UserId);
            return View(expenses);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenses = await _context.Expenses
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

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expenses = await _context.Expenses.FindAsync(id);
            if (expenses != null)
            {
                _context.Expenses.Remove(expenses);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpensesExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
