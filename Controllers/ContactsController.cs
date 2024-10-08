using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _BranchId = 1;
        public ContactsController(ERManagerContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.Contact.Include(c => c.Branch).Include(c => c.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Include(c => c.Branch)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            // Pass the enum list to the view
            ViewBag.ContactType = Enum.GetValues(typeof(ContactType))
                                      .Cast<ContactType>()
                                      .Select(ct => new SelectListItem
                                      {
                                          Value = ((int)ct).ToString(),
                                          Text = ct.ToString()
                                      });
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Phone,Email,ContactType,CreatedAt")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                contact.IsActive = true;
                contact.UserId = _LogUserId;
                contact.UpdateAt = DateTime.Now;
                contact.BranchId = _BranchId;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Pass the enum list to the view
            ViewBag.ContactType = Enum.GetValues(typeof(ContactType))
                                      .Cast<ContactType>()
                                      .Select(ct => new SelectListItem
                                      {
                                          Value = ((int)ct).ToString(),
                                          Text = ct.ToString()
                                      });
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            // Populate the ContactType dropdown
            ViewBag.ContactType = Enum.GetValues(typeof(ContactType))
                                      .Cast<ContactType>()
                                      .Select(ct => new SelectListItem
                                      {
                                          Value = ((int)ct).ToString(),
                                          Text = ct.ToString()
                                      });
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Phone,Email,IsActive,ContactType,CreatedAt")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contact.UserId = _LogUserId;
                    contact.UpdateAt = DateTime.Now;
                    contact.BranchId = _BranchId;
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            // Populate the ContactType dropdown
            ViewBag.ContactType = Enum.GetValues(typeof(ContactType))
                                      .Cast<ContactType>()
                                      .Select(ct => new SelectListItem
                                      {
                                          Value = ((int)ct).ToString(),
                                          Text = ct.ToString()
                                      });
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .Include(c => c.Branch)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact != null)
            {
                _context.Contact.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id);
        }


        // ContactController.cs

        public async Task<IActionResult> ToggleActiveStatus(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            // Toggle the IsActive status
            contact.IsActive = !contact.IsActive;
            _context.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the contact list
        }

    }
}
