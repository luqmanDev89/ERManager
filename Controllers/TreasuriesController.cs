using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Treasury;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    public class TreasuriesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _BranchId = 1;

        public TreasuriesController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: Treasuries
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.Treasuries.Include(t => t.Branch).Include(t => t.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: Treasuries/Details/5
        public async Task<IActionResult> Details(int? id)
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
                // Load User and Branch
                var user = await _context.Users.FindAsync(_LogUserId);
                var branch = await _context.Branches.FindAsync(_BranchId);

                if (user == null || branch == null)
                {
                    // Handle the case when User or Branch is not found
                    ModelState.AddModelError(string.Empty, "User or Branch not found.");
                    return View(model);
                }

                // Create a new Treasury instance
                var treasury = new Treasury
                {
                    Name = model.Name,
                    Description = model.Description,
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
                Description = treasury.Description,
                CreatedAt = treasury.CreatedAt,
               
            };

            return View(treasuryViewModel);
        }


        // POST: Treasuries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreasuryId, Name, Description, CreatedAt")] TreasuryViewModel treasuryViewModel)
        {
            if (id != treasuryViewModel.TreasuryId)
            {
                return NotFound();
            }

            // Load User and Branch
            var user = await _context.Users.FindAsync(_LogUserId);
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
                Description = treasuryViewModel.Description,
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
            if (treasury != null)
            {
                _context.Treasuries.Remove(treasury);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreasuryExists(int id)
        {
            return _context.Treasuries.Any(e => e.TreasuryId == id);
        }
    }
}
