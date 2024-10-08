using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    public class ExpensesCategoriesController : Controller
    {
        private readonly ERManagerContext _context;
        public ExpensesCategoriesController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: ExpensesCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExpensesCategory.ToListAsync());
        }

        // GET: ExpensesCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expensesCategory = await _context.ExpensesCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expensesCategory == null)
            {
                return NotFound();
            }

            return View(expensesCategory);
        }

        // GET: ExpensesCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExpensesCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ExpensesCategory expensesCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expensesCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expensesCategory);
        }

        // GET: ExpensesCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expensesCategory = await _context.ExpensesCategory.FindAsync(id);
            if (expensesCategory == null)
            {
                return NotFound();
            }
            return View(expensesCategory);
        }

        // POST: ExpensesCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ExpensesCategory expensesCategory)
        {
            if (id != expensesCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expensesCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpensesCategoryExists(expensesCategory.Id))
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
            return View(expensesCategory);
        }

        // GET: ExpensesCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expensesCategory = await _context.ExpensesCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expensesCategory == null)
            {
                return NotFound();
            }

            return View(expensesCategory);
        }

        // POST: ExpensesCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expensesCategory = await _context.ExpensesCategory.FindAsync(id);
            if (expensesCategory != null)
            {
                _context.ExpensesCategory.Remove(expensesCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpensesCategoryExists(int id)
        {
            return _context.ExpensesCategory.Any(e => e.Id == id);
        }
    }
}
