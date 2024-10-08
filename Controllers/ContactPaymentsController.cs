using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    public class ContactPaymentsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _LogUserId = 1;
        private readonly int _DefaultCurrencyId = 1;

        public ContactPaymentsController(ERManagerContext context)
        {
            _context = context;
        }


        // GET: ContactPayments
        public async Task<IActionResult> Index()
        {
            var eRManagerContext = _context.ContactPayment.Include(c => c.Contact).Include(c => c.Currency).Include(c => c.Treasury).Include(c => c.User);
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: ContactPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactPayment = await _context.ContactPayment
                .Include(c => c.Contact)
                .Include(c => c.Currency)
                .Include(c => c.Treasury)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactPayment == null)
            {
                return NotFound();
            }

            return View(contactPayment);
        }

        // GET: ContactPayments/Create
        public async Task<IActionResult> Create()
        {
            var contacts = await _context.Contact.ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.ToListAsync();

            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.PaymentTypes = new SelectList(
                Enum.GetValues(typeof(PaymentType))
                    .Cast<PaymentType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");

            ViewBag.ContactId = new SelectList(contacts, "Id", "Name");
            ViewBag.TreasuryId = new SelectList(treasuries, "TreasuryId", "Name");

            return View();
        }

        // POST: ContactPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Notes,PaymentType,CreatedAt,ContactId,TreasuryId")] ContactPayment contactPayment)
        {
          
            if (ModelState.IsValid)
            {
                contactPayment.CurrencyId = _DefaultCurrencyId;
                contactPayment.UserId = _LogUserId;
                contactPayment.UpdateAt = DateTime.Now;
                _context.Add(contactPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var contacts = await _context.Contact.ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.ToListAsync();

            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.PaymentTypes = new SelectList(
                Enum.GetValues(typeof(PaymentType))
                    .Cast<PaymentType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");

            ViewBag.ContactId = new SelectList(contacts, "Id", "Name");
            ViewBag.TreasuryId = new SelectList(treasuries, "TreasuryId", "Name");

            return View(contactPayment);
        }

        // GET: ContactPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactPayment = await _context.ContactPayment.FindAsync(id);
            if (contactPayment == null)
            {
                return NotFound();
            }
            var contacts = await _context.Contact.ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.ToListAsync();

            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.PaymentTypes = new SelectList(
                Enum.GetValues(typeof(PaymentType))
                    .Cast<PaymentType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");

            ViewBag.ContactId = new SelectList(contacts, "Id", "Name");
            ViewBag.TreasuryId = new SelectList(treasuries, "TreasuryId", "Name");
            return View(contactPayment);
        }

        // POST: ContactPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Notes,PaymentType,CreatedAt,ContactId,TreasuryId")] ContactPayment contactPayment)
        {
            if (id != contactPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contactPayment.CurrencyId = _DefaultCurrencyId;
                    contactPayment.UserId = _LogUserId;
                    contactPayment.UpdateAt = DateTime.Now;
                    _context.Update(contactPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactPaymentExists(contactPayment.Id))
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
            var contacts = await _context.Contact.ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.ToListAsync();

            // Create SelectLists for Payment Types, Contacts, and Treasuries
            ViewBag.PaymentTypes = new SelectList(
                Enum.GetValues(typeof(PaymentType))
                    .Cast<PaymentType>()
                    .Select(e => new { Id = e, Name = e.ToString() }),
                "Id", "Name");

            ViewBag.ContactId = new SelectList(contacts, "Id", "Name");
            ViewBag.TreasuryId = new SelectList(treasuries, "TreasuryId", "Name");
            return View(contactPayment);
        }

        // GET: ContactPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactPayment = await _context.ContactPayment
                .Include(c => c.Contact)
                .Include(c => c.Currency)
                .Include(c => c.Treasury)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactPayment == null)
            {
                return NotFound();
            }

            return View(contactPayment);
        }

        // POST: ContactPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactPayment = await _context.ContactPayment.FindAsync(id);
            if (contactPayment != null)
            {
                _context.ContactPayment.Remove(contactPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactPaymentExists(int id)
        {
            return _context.ContactPayment.Any(e => e.Id == id);
        }
    }
}
