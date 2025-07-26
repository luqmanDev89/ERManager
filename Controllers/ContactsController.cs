using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ContactsController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _BranchId = 1;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ContactsController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
            // Accessing the session here where HttpContext is available
            string selectedBranchIdStr = _httpContextAccessor.HttpContext?.Session.GetString("SelectedBranch") ?? "1";
            if (selectedBranchIdStr != null)
            {
                _BranchId = int.Parse(selectedBranchIdStr);
            }
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Contacts
        public async Task<IActionResult> Index(string? ContactType, string? Name)
        {
            var eRManagerContext = _context.Contact
                    .AsNoTracking() // Disable change tracking for read-only operation
                    .Include(c => c.Branch)
                    .Include(c => c.User)
                    .Include(c => c.ContactPayments)
                    .Include(c => c.SaleInvoices)
                        .ThenInclude(s => s.SaleInvoiceItems) // Include SaleInvoiceItems in SaleInvoices
                    .Include(c => c.PurchaseInvoices)
                        .ThenInclude(p => p.PurchaseInvoiceItems) // Include PurchaseInvoiceItems in PurchaseInvoices
                    .Where(c => c.BranchId == _BranchId)
                    .AsQueryable();

            // Filter by ContactType if provided
            if (!string.IsNullOrWhiteSpace(ContactType))
            {
                if (Enum.TryParse(ContactType, out ContactType contactTypeEnum))
                {
                    eRManagerContext = eRManagerContext.Where(d => d.ContactType == contactTypeEnum);
                }
            }

            // Filter by Name if provided
            if (!string.IsNullOrWhiteSpace(Name))
            {
                eRManagerContext = eRManagerContext.Where(d => d.Name.Contains(Name) || d.Phone.Contains(Name));
            }

            // Load the data into memory
            var contactsList = await eRManagerContext.ToListAsync();

            // Perform calculation in memory to avoid translation issues
            var SupplierBalance = contactsList.Where(d => d.ContactType == Models.ContactType.Supplier).Sum(d =>
                d.PurchaseInvoices.Sum(pi => pi.InvoiceTotal)
                - d.ContactPayments.Where(cp => cp.PaymentType == PaymentType.Outflow).Sum(cp => cp.Amount)
            );
            // Perform calculation in memory to avoid translation issues
            var CustomerBalance = contactsList.Where(d => d.ContactType == Models.ContactType.Customer).Sum(d =>
                d.SaleInvoices.Sum(pi => pi.InvoiceTotal)
                - d.ContactPayments.Where(cp => cp.PaymentType == PaymentType.Inflow).Sum(cp => cp.Amount)
            );

            ViewBag.SupplierTotal = SupplierBalance;
            ViewBag.CustomerTotal = CustomerBalance;
            ViewBag.Total = CustomerBalance - SupplierBalance;

            return View(contactsList);
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
            // Manual validation for the bound properties
            if (string.IsNullOrWhiteSpace(contact.Name) ||
                string.IsNullOrWhiteSpace(contact.Phone) ||
                contact.CreatedAt == default(DateTime))
            {
                ModelState.AddModelError(string.Empty, "Required fields are missing or invalid.");
            }

            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                contact.IsActive = true;
                contact.UserId = userId;
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

            // Manual validation for the bound properties
            if (string.IsNullOrWhiteSpace(contact.Name) ||
                string.IsNullOrWhiteSpace(contact.Phone) ||
                contact.CreatedAt == default(DateTime))
            {
                ModelState.AddModelError(string.Empty, "Required fields are missing or invalid.");
            }

            else
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    contact.UserId = userId;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contact.FindAsync(id);

            if (contact == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "Contact not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Contact.Remove(contact);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "Contact deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the contact.";
                return RedirectToAction("Error", "Home");
            }
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

        public async Task<IActionResult> ContactBalance(int contactId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Fetch the contact with related data, using AsNoTracking for better performance
                var selectedContact = await _context.Contact
                    .AsNoTracking() // No tracking for this query
                    .Include(c => c.ContactPayments)
                    .Include(c => c.SaleInvoices)
                        .ThenInclude(s => s.SaleInvoiceItems) // Include SaleInvoiceItems within SaleInvoices
                    .Include(c => c.PurchaseInvoices)
                        .ThenInclude(p => p.PurchaseInvoiceItems) // Include PurchaseInvoiceItems within PurchaseInvoices
                    .FirstOrDefaultAsync(c => c.Id == contactId && c.BranchId == _BranchId);


                if (selectedContact == null)
                {
                    return NotFound($"Customer with ID {contactId} not found.");
                }

                // Calculate payments and invoices before the start date
                var beforePaymentsInflow = selectedContact.ContactPayments?
                    .Where(payment => payment.CreatedAt < startDate && payment.PaymentType == PaymentType.Inflow)
                    .Sum(payment => (double?)payment.Amount) ?? 0;

                var beforePaymentsOutflow = selectedContact.ContactPayments?
                    .Where(payment => payment.CreatedAt < startDate && payment.PaymentType == PaymentType.Outflow)
                    .Sum(payment => (double?)payment.Amount) ?? 0;

                var invoicesBeforeStartDate = selectedContact.SaleInvoices?
                    .Where(invoice => invoice.CreatedAt < startDate)
                    .Sum(d => d.InvoiceTotal) ?? 0;

                var purchaseInvoicesBeforeStartDate = selectedContact.PurchaseInvoices?
                    .Where(invoice => invoice.CreatedAt < startDate)
                    .Sum(d => d.InvoiceTotal) ?? 0;

                // Calculate the opening balance
                double openingBalance = selectedContact.ContactType == ContactType.Customer
                    ? invoicesBeforeStartDate - beforePaymentsInflow
                    : beforePaymentsOutflow - purchaseInvoicesBeforeStartDate;

                // Create an opening balance entry
                var openingBalanceEntry = new BalanceViewModel
                {
                    CustomerId = contactId,
                    CustomerName = selectedContact.Name,
                    InvoiceId = 0, // Opening balance indicator
                    Type = "حسابی پێشووتر",
                    TotalAmount = 0,
                    Description = "باڵانسی پێش ئەم بەروارە",
                    CreatedAt = startDate.AddDays(-1), // Date before start date
                    Balance = openingBalance,
                    creditor = 0,
                    debtor = 0
                };

                // Fetch purchase invoices, sales invoices, and payments within date range
                var purchaseInvoiceViewModels = selectedContact.PurchaseInvoices?
                    .Where(invoice => invoice.CreatedAt >= startDate && invoice.CreatedAt <= endDate)
                    .Select(s => new BalanceViewModel
                    {
                        CustomerId = contactId,
                        CustomerName = selectedContact.Name,
                        InvoiceId = s.Id,
                        Type = "کڕین",
                        TotalAmount = s.InvoiceTotal,
                        Description = s.Notes ?? "No description",
                        CreatedAt = s.CreatedAt ?? DateTime.Now,
                        debtor = s.InvoiceTotal,
                        creditor = 0
                    }).ToList() ?? new List<BalanceViewModel>();

                var salesInvoiceViewModels = selectedContact.SaleInvoices?
                    .Where(invoice => invoice.CreatedAt >= startDate && invoice.CreatedAt <= endDate)
                    .Select(s => new BalanceViewModel
                    {
                        CustomerId = contactId,
                        CustomerName = selectedContact.Name,
                        InvoiceId = s.Id,
                        Type = "فڕۆشتن",
                        TotalAmount = s.InvoiceTotal,
                        Description = s.Notes ?? "No description",
                        CreatedAt = s.CreatedAt ?? DateTime.Now,
                        creditor = s.InvoiceTotal,
                        debtor = 0
                    }).ToList() ?? new List<BalanceViewModel>();

                var contactPaymentsInflow = selectedContact.ContactPayments?
                    .Where(payment => payment.CreatedAt >= startDate && payment.CreatedAt <= endDate && payment.PaymentType == PaymentType.Inflow)
                    .Select(payment => new BalanceViewModel
                    {
                        CustomerId = contactId,
                        CustomerName = selectedContact.Name,
                        InvoiceId = payment.Id,
                        Type = "پارەوەرگرتن",
                        TotalAmount = payment.Amount,
                        Description = payment.Notes ?? "No notes",
                        CreatedAt = payment.CreatedAt,
                        debtor = payment.Amount,
                        creditor = 0
                    }).ToList() ?? new List<BalanceViewModel>();

                var contactPaymentsOutflow = selectedContact.ContactPayments?
                    .Where(payment => payment.CreatedAt >= startDate && payment.CreatedAt <= endDate && payment.PaymentType == PaymentType.Outflow)
                    .Select(payment => new BalanceViewModel
                    {
                        CustomerId = contactId,
                        CustomerName = selectedContact.Name,
                        InvoiceId = payment.Id,
                        Type = "پارەدان",
                        TotalAmount = payment.Amount,
                        Description = payment.Notes ?? "No notes",
                        CreatedAt = payment.CreatedAt,
                        creditor = payment.Amount,
                        debtor = 0
                    }).ToList() ?? new List<BalanceViewModel>();

                // Combine transactions with the opening balance and sort by date
                var transactions = new List<BalanceViewModel> { openingBalanceEntry }
                    .Concat(salesInvoiceViewModels)
                    .Concat(purchaseInvoiceViewModels)
                    .Concat(contactPaymentsInflow)
                    .Concat(contactPaymentsOutflow)
                    .OrderBy(t => t.CreatedAt)
                    .ToList();

                // Calculate the running balance
                double runningBalance = openingBalance;
                foreach (var transaction in transactions)
                {
                    runningBalance += transaction.creditor - transaction.debtor;
                    transaction.Balance = runningBalance;
                }

                // Prepare ViewBag data for the view
                ViewBag.Contacts = await _context.Contact.Where(d => d.BranchId == _BranchId).ToListAsync();
                ViewBag.selectedContact = selectedContact;
                ViewBag.TotalPayments = transactions.Sum(d => d.creditor);
                ViewBag.TotalSales = transactions.Sum(d => d.debtor);
                ViewBag.CurrentBalance = runningBalance;
                ViewBag.endDate = endDate;
                ViewBag.startDate = startDate;
                ViewBag.contactId = contactId;
                ViewBag.openingBalance = openingBalance;
                ViewData["Title"] = "Customer Balance";

                return View(transactions);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error occurred while fetching customer balance: {ex.Message}");
                return StatusCode(500, "Internal server error occurred while processing your request. Please try again later.");
            }
        }

    }
}
