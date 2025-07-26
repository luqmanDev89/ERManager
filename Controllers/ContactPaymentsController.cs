using ERManager.Data;
using ERManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ContactPaymentsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContactPaymentsController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // Accessing the session here where HttpContext is available
            string selectedBranchIdStr = _httpContextAccessor.HttpContext?.Session.GetString("SelectedBranch") ?? "1";
            if (selectedBranchIdStr != null)
            {
                _BranchId = int.Parse(selectedBranchIdStr);
            }
            _context = context ?? throw new ArgumentNullException(nameof(context));
            // Set default currency ID based on available currencies in the database
            var defaultCurrency = _context.Currency.FirstOrDefault();
            _DefaultCurrencyId = defaultCurrency?.CurrencyId ?? 1;
        }


        // GET: ContactPayments
        public async Task<IActionResult> Index(string? PaymentType, int? ContactId, int? TreasuryId, DateTime? startDate, DateTime? endDate, string? Notes)
        {

            var eRManagerContext = _context.ContactPayment
              .Include(c => c.Contact)
                  .ThenInclude(c => c.Branch)
              .Include(c => c.Currency)
              .Include(c => c.Treasury)
              .Include(c => c.User)
              .AsQueryable();

            // Apply the branch filter
            eRManagerContext = eRManagerContext.Where(d => d.Contact.BranchId == _BranchId);
            eRManagerContext = eRManagerContext.Where(d => d.Treasury.BranchId == _BranchId);
            if (ContactId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.ContactId == ContactId);
            }
            if (TreasuryId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.TreasuryId == TreasuryId);
            }

            if (!string.IsNullOrEmpty(Notes))
            {
                eRManagerContext = eRManagerContext.Where(e => e.Notes != null && e.Notes.Contains(Notes));
            }
            if (!string.IsNullOrEmpty(PaymentType))
            {
                // Try to parse the string to the PaymentType enum
                if (Enum.TryParse(PaymentType, out PaymentType parsedPaymentType))
                {
                    eRManagerContext = eRManagerContext.Where(e => e.PaymentType == parsedPaymentType);
                }
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                eRManagerContext = eRManagerContext.Where(e => e.CreatedAt.Date >= startDate && e.CreatedAt.Date <= endDate);
            }
            else
            {
                eRManagerContext = eRManagerContext.Where(e => e.CreatedAt.Date >= DateTime.Now.Date && e.CreatedAt.Date <= DateTime.Now.Date);
            }

            // Pass categories to populate the Category filter dropdown
            ViewBag.Notes = Notes;
            ViewBag.TreasuryId = TreasuryId;
            ViewBag.ContactId = ContactId;
            ViewBag.startDate = startDate ?? DateTime.Now;
            ViewBag.endDate = endDate ?? DateTime.Now;
            ViewBag.Contacts = _context.Contact.ToList();
            ViewBag.Treasuries = _context.Treasuries.ToList();
            ViewBag.totalInflow = eRManagerContext.Where(d => d.PaymentType == Models.PaymentType.Inflow).ToList().Sum(d => (double)d.Amount);
            ViewBag.totalOutflow = eRManagerContext.Where(d => d.PaymentType == Models.PaymentType.Outflow).ToList().Sum(d => (double)d.Amount);
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
            var contacts = await _context.Contact.Where(d => d.BranchId == _BranchId&&d.ContactType==ContactType.Supplier).ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

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

        // GET: ContactPayments/Create
        public async Task<IActionResult> CreateInFlow()
        {
            var contacts = await _context.Contact.Where(d => d.BranchId == _BranchId && d.ContactType == ContactType.Customer).ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

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
            // Manual validation for the bound properties
            if (contactPayment.Amount <= 0 ||
               string.IsNullOrWhiteSpace(contactPayment.PaymentType.ToString()) ||
               string.IsNullOrWhiteSpace(contactPayment.CreatedAt.ToString()) ||
                contactPayment.ContactId <= 0 ||
                contactPayment.TreasuryId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Required fields are missing or invalid.");
            }

            else
            {
                // Retrieve user ID from claims inside the action method
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                contactPayment.CurrencyId = _DefaultCurrencyId;
                contactPayment.UserId = userId;
                contactPayment.UpdateAt = DateTime.Now;
                _context.Add(contactPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var contacts = await _context.Contact.Where(d => d.BranchId == _BranchId).ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

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
            var contacts = await _context.Contact.Where(d => d.BranchId == _BranchId).ToListAsync(); // Ensure this is the correct DbSet
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).Where(d => d.BranchId == _BranchId).ToListAsync();

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Notes,PaymentType,CreatedAt,ContactId,TreasuryId,PurchaseInvoiceId")] ContactPayment contactPayment)
        {
            if (id != contactPayment.Id)
            {
                return NotFound();
            }

            // Manually validate fields

            else if (contactPayment.ContactId == 0)
            {
                ModelState.AddModelError("ContactId", "Contact is required.");
            }

            else if (contactPayment.TreasuryId == 0)
            {
                ModelState.AddModelError("TreasuryId", "Treasury is required.");
            }

            else if (contactPayment.CreatedAt == default)
            {
                ModelState.AddModelError("CreatedAt", "Creation date is required.");
            }


            // Check if ModelState is valid after manual validation
            else 
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(userId))
                    {
                        // Handle missing userId if necessary (e.g., redirect, log out, etc.)
                        return RedirectToAction("Login", "Account");
                    }

                    contactPayment.CurrencyId = _DefaultCurrencyId;
                    contactPayment.UserId = userId;
                    contactPayment.UpdateAt = DateTime.Now;

                    // Update the contact payment in the database
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

                // After saving, redirect to Index or any other appropriate action
                return RedirectToAction(nameof(Index));
            }

            // Fetch contacts and treasuries based on branch context
            var contacts = await _context.Contact.Where(d => d.BranchId == _BranchId).ToListAsync();
            var treasuries = await _context.Treasuries.Where(d => d.BranchId == _BranchId).ToListAsync();

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var contactPayment = await _context.ContactPayments
                    .FirstOrDefaultAsync(cp => cp.Id == id);

                if (contactPayment != null)
                {
                    // Step 1: Find the related PurchaseInvoice and set ContactPaymentId to null
                    var relatedPurchaseInvoice = await _context.PurchaseInvoices
                        .FirstOrDefaultAsync(pi => pi.ContactPaymentId == contactPayment.Id);

                    // Step 1: Find the related SaleInvoice and set ContactPaymentId to null
                    var relatedSaleInvoice = await _context.SaleInvoice
                        .FirstOrDefaultAsync(pi => pi.ContactPaymentId == contactPayment.Id);

                    var relatedPurchaseInvoiceItem = await _context.PurchaseInvoiceItems
                       .FirstOrDefaultAsync(pi => pi.ContactPaymentId == contactPayment.Id);

                    if (relatedPurchaseInvoice != null)
                    {
                        // Set ContactPaymentId to null for the related PurchaseInvoice
                        relatedPurchaseInvoice.ContactPaymentId = null;
                    }
                    if (relatedPurchaseInvoiceItem != null)
                    {
                        // Set ContactPaymentId to null for the related PurchaseInvoice
                        relatedPurchaseInvoiceItem.ContactPaymentId = null;
                    }

                    if (relatedSaleInvoice != null)
                    {
                        // Set ContactPaymentId to null for the related PurchaseInvoice
                        relatedSaleInvoice.ContactPaymentId = null;
                    }

                    // Step 2: Remove the ContactPayment
                    _context.ContactPayments.Remove(contactPayment);

                    // Step 3: Save changes to the database
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception if needed (e.g., to a logging service)
                TempData["ErrorMessage"] = "An error occurred while deleting the contact payment. Please try again.";
                return RedirectToAction("Error", "Home");
            }
        }


        private bool ContactPaymentExists(int id)
        {
            return _context.ContactPayment.Any(e => e.Id == id);
        }
    }
}
