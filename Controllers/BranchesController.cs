﻿using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class BranchesController : Controller
    {
        private readonly ERManagerContext _context;

        public BranchesController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Branches.ToListAsync());
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.CreatedAt = DateTime.Now;
                branch.UpdatedAt = DateTime.Now;
                branch.IsDefault = false;
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,CreatedAt,UpdatedAt,IsDefault")] Branch branch)
        {
            if (id != branch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.Id))
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
            return View(branch);
        }

        // GET: Branches/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "Contact not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "branch deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the branch.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }


        [HttpPost]
        public async Task<IActionResult> SetAsDefault(int id)
        {
            // Fetch all branches
            var branches = await _context.Branches.ToListAsync();

            // Find the selected branch
            var selectedBranch = branches.FirstOrDefault(b => b.Id == id);
            if (selectedBranch == null)
            {
                return NotFound();
            }

            // Set all branches' IsDefault to false
            foreach (var branch in branches)
            {
                branch.IsDefault = false;
            }

            // Set the selected branch's IsDefault to true
            selectedBranch.IsDefault = true;

            // Save changes
            await _context.SaveChangesAsync();

            // Redirect back to the index page
            return RedirectToAction(nameof(Index));
        }



        // Method to check and create default branch if not exists
        private void CreateDefaultBranchIfNeeded()
        {
            // Check if there are any branches in the database
            var branchCount = _context.Branches.Count();
            if (branchCount == 0)
            {
                // If no branches exist, create a default branch
                var defaultBranch = new Branch
                {
                    Name = "لقی یەکەم",
                    Location = "Default Location",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDefault = true // Set this branch as the default
                };

                _context.Branches.Add(defaultBranch);
                _context.SaveChanges();
            }
        }

    }
}
