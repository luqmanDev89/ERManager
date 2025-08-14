using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PurchaseInvoicesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId = 1;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PurchaseInvoicesController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: PurchaseInvoices
        public async Task<IActionResult> Index(int? ContactId, DateTime? startDate, DateTime? endDate, string? Notes, bool? IsPaid, bool? IsNotPaid)
        {
            var eRManagerContext = _context.PurchaseInvoices.Where(d => d.BranchId == _BranchId)
                .Include(p => p.Branch)
                .Include(p => p.Contact)
                .Include(p => p.Currency)
                .Include(p => p.User)
                .Include(p => p.PurchaseInvoiceItems)
                .AsQueryable();

            // Filter by ContactId if provided
            if (ContactId.HasValue)
            {
                eRManagerContext = eRManagerContext.Where(e => e.ContactId == ContactId);
            }

            // Filter by Notes if provided
            if (!string.IsNullOrEmpty(Notes))
            {
                eRManagerContext = eRManagerContext.Where(e => e.Notes != null && e.Notes.Contains(Notes));
            }

            // Filter by Date Range if provided
            if (startDate.HasValue && endDate.HasValue)
            {
                eRManagerContext = eRManagerContext.Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);
            }
            else
            {
                eRManagerContext = eRManagerContext.Where(e => e.CreatedAt.Value.Date >= DateTime.Now.Date && e.CreatedAt.Value.Date <= DateTime.Now.Date);
            }

            // Filter by Payment Status if provided
            if (IsPaid.HasValue && IsPaid.Value)
            {
                eRManagerContext = eRManagerContext.Where(e => e.ContactPaymentId != null);
            }
            if (IsNotPaid.HasValue && IsNotPaid.Value)
            {
                eRManagerContext = eRManagerContext.Where(e => e.ContactPaymentId == null);
            }

            // Pass contacts for the dropdown filter
            ViewBag.Contacts = await _context.Contact.Where(d => d.BranchId == _BranchId).ToListAsync();

            // Execute the query and get the filtered results
            var filteredInvoices = await eRManagerContext.ToListAsync();

            // Calculate the total amount of filtered invoices
            var totalAmount = filteredInvoices.Sum(d => d.InvoiceTotal);

            // Pass data to the ViewBag
            ViewBag.TotalAmount = totalAmount; // Capital 'T' for consistency
            ViewBag.ContactId = ContactId;
            ViewBag.Notes = Notes;
            ViewBag.StartDate = startDate ?? DateTime.Now.Date; // Use consistent casing
            ViewBag.EndDate = endDate ?? DateTime.Now.Date; // Use consistent casing
            ViewBag.IsPaid = IsPaid;
            ViewBag.IsNotPaid = IsNotPaid;

            return View(filteredInvoices);
        }


        // GET: PurchaseInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoice = await _context.PurchaseInvoices
                .Include(p => p.Branch)
                .Include(p => p.Contact)
                .Include(p => p.Currency)
                .Include(p => p.User)
                .Include(p => p.PurchaseInvoiceItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseInvoice == null)
            {
                return NotFound();
            }

            return View(purchaseInvoice);
        }

        // GET: PurchaseInvoices/Create
        public IActionResult Create()
        {
            ViewData["ContactId"] = new SelectList(_context.Contact.Where(d => d.ContactType == ContactType.Supplier && d.BranchId == _BranchId), "Id", "Name");
            return View();
        }

        // POST: PurchaseInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,ContactId")] PurchaseInvoice purchaseInvoice)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            purchaseInvoice.UserId = userId;
            purchaseInvoice.CurrencyId = _DefaultCurrencyId;
            purchaseInvoice.BranchId = _BranchId;
            purchaseInvoice.UpdateAt = DateTime.Now;
            purchaseInvoice.Tax = 0;
            purchaseInvoice.Discount = 0;
            purchaseInvoice.InvoiceNumber = string.Empty;
            purchaseInvoice.IsPaid = false;
            purchaseInvoice.CreatedAt = purchaseInvoice.CreatedAt ?? DateTime.Now;

            if (
                purchaseInvoice.CreatedAt.HasValue &&
                purchaseInvoice.ContactId > 0 &&
                !string.IsNullOrEmpty(purchaseInvoice.UserId) &&
                purchaseInvoice.BranchId > 0 &&
                purchaseInvoice.CurrencyId > 0)
            {
                try
                {
                    _context.Add(purchaseInvoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = purchaseInvoice.Id });
                }
                catch (Exception ex)
                {
                    var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact support. Error: " + ex.Message + " Inner Error: " + innerExceptionMessage);
                }
            }
            else
            {
                ModelState.AddModelError("", "Please ensure all required fields are filled out correctly.");
            }

            ViewData["ContactId"] = new SelectList(_context.Contact
                .Where(d => d.BranchId == _BranchId && d.ContactType == ContactType.Supplier), "Id", "Name", purchaseInvoice.ContactId);

            return View(purchaseInvoice);
        }


        //InvoiceDetails
        // GET: PurchaseInvoices/Edit/5
        public async Task<IActionResult> InvoiceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoice = await _context.PurchaseInvoices
                .AsNoTracking()
                .Include(d => d.Contact)
                .Include(p => p.PurchaseInvoiceItems)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseInvoice == null)
            {
                return NotFound();
            }

            return View(purchaseInvoice);
        }


        // GET: PurchaseInvoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the specific PurchaseInvoice with necessary includes
            var purchaseInvoice = await _context.PurchaseInvoices
                .Where(d => d.BranchId == _BranchId)
                .Include(d => d.Contact)
                .Include(p => p.PurchaseInvoiceItems)
                    .ThenInclude(item => item.Product) // Only include Product if required for view
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseInvoice == null)
            {
                return NotFound();
            }

            var contactPurchaseInvoices = await _context.PurchaseInvoices
                .Where(pi => pi.ContactId == purchaseInvoice.ContactId && pi.BranchId == _BranchId)
                .AsNoTracking()
                .Include(d => d.PurchaseInvoiceItems)
                .ToListAsync(); // Fetch data into memory

            var totalInvoiceAmount = contactPurchaseInvoices.Sum(pi => pi.InvoiceTotal);


            var totalOutflowPayments = await _context.ContactPayments
                .Where(c => c.ContactId == purchaseInvoice.ContactId && c.PaymentType == PaymentType.Outflow)
                .SumAsync(c => c.Amount);

            var balance = totalOutflowPayments - totalInvoiceAmount;

            // Populate dropdowns for contact and products
            ViewData["ContactId"] = new SelectList(_context.Contact
                .Where(d => d.ContactType == ContactType.Supplier && d.BranchId == _BranchId),
                "Id", "Name", purchaseInvoice.ContactId);

            ViewBag.Products = await _context.Products.ToListAsync();

            // Pass the PurchaseInvoice and calculated balance to the view
            ViewBag.Balance = balance;

            return View(purchaseInvoice);
        }



        // POST: PurchaseInvoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InvoiceNumber,Notes,Discount,Tax,CreatedAt,ContactId")] PurchaseInvoice purchaseInvoice)
        {
            // Check if the ID from the route matches the ID in the saleInvoice object
            if (id != purchaseInvoice.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            purchaseInvoice.UserId = userId;
            purchaseInvoice.CurrencyId = _DefaultCurrencyId;
            purchaseInvoice.BranchId = _BranchId;
            purchaseInvoice.UpdateAt = DateTime.Now;

            try
            {
                // Update the saleInvoice in the context
                _context.Update(purchaseInvoice);
                await _context.SaveChangesAsync();

                // Set the success message in TempData
                TempData["SuccessMessage"] = "Invoice updated successfully!";

                // Redirect to the same page (or the desired page)
                return RedirectToAction("Edit", new { id = purchaseInvoice.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!PurchaseInvoiceExists(purchaseInvoice.Id))
                {
                    return NotFound();
                }
                else
                {
                    // If the update fails due to a concurrency issue, throw the exception
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed (optional)
                // _logger.LogError(ex, "Error updating invoice");

                // You can also set an error message in TempData if needed
                TempData["ErrorMessage"] = "Error updating invoice: " + ex.Message;

                // Redirect back to the same page
                return RedirectToAction("Edit", new { id = purchaseInvoice.Id });
            }
        }

        // GET: PurchaseInvoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseInvoice = await _context.PurchaseInvoices.Where(d => d.BranchId == _BranchId)
                .Include(p => p.Branch)
                .Include(p => p.Contact)
                .Include(p => p.Currency)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseInvoice == null)
            {
                return NotFound();
            }

            return View(purchaseInvoice);
        }

        // POST: PurchaseInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseInvoice = await _context.PurchaseInvoices.FindAsync(id);
            if (purchaseInvoice == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "purchaseInvoice not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.PurchaseInvoice.Remove(purchaseInvoice);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "purchaseInvoice deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the purchaseInvoice.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool PurchaseInvoiceExists(int id)
        {
            return _context.PurchaseInvoices.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult AddItem(int InvoiceId, int NewProductId, double NewProductQuantity, double NewProductUnitPrice)
        {
            // Retrieve the invoice from the database
            var invoice = _context.PurchaseInvoices.Where(d => d.BranchId == _BranchId).Include(i => i.PurchaseInvoiceItems)
                                                    .FirstOrDefault(i => i.Id == InvoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            // Create a new item
            var item = new PurchaseInvoiceItem
            {
                PurchaseInvoiceId = InvoiceId,
                ProductId = NewProductId,
                Quantity = NewProductQuantity,
                UnitPrice = NewProductUnitPrice,
                TaxPresent = 0
            };

            // Add the item to the invoice
            invoice.PurchaseInvoiceItems.Add(item);

            // Save changes to the database
            _context.SaveChanges();

            // Redirect back to the edit page
            return RedirectToAction("Edit", new { id = InvoiceId });
        }


        [HttpPost]
        public IActionResult EditItem(int id, string field, string value)
        {
            // Fetch the item by id
            var item = _context.PurchaseInvoiceItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            // Update the appropriate field based on the `field` parameter
            switch (field)
            {
                case "Quantity":
                    item.Quantity = double.Parse(value);
                    break;
                case "UnitPrice":
                    item.UnitPrice = double.Parse(value);
                    break;
                case "TaxPresent":
                    item.TaxPresent = double.Parse(value);
                    break;
                default:
                    return BadRequest("Invalid field specified.");
            }

            // Save changes to the database
            _context.SaveChanges();

            return Ok(); // Or return some relevant data if needed
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            // Find the item by ID
            var item = _context.PurchaseInvoiceItems.Find(id);
            if (item == null)
            {
                return NotFound(); // Return 404 if the item is not found
            }

            // Remove the item
            _context.PurchaseInvoiceItems.Remove(item);
            _context.SaveChanges(); // Save changes to the database

            return Ok(); // Return a 200 OK response
        }


        public ActionResult PurchaseDashboard()
        {
            // Retrieve suppliers with their related PurchaseInvoices and ContactPayments
            var suppliers = _context.Contact
                .Where(d => d.BranchId == _BranchId && d.ContactType == ContactType.Supplier)
                .Include(c => c.PurchaseInvoices)
                    .ThenInclude(pi => pi.PurchaseInvoiceItems)
                .Include(c => c.ContactPayments)
                .Include(c => c.User)
                .ToList(); // Retrieve data in one go

            // Calculate today's total purchase
            var ToDayTotalPurchase = suppliers
                .SelectMany(supplier => supplier.PurchaseInvoices
                    .Where(d => d.CreatedAt.HasValue && d.CreatedAt.Value.Date == DateTime.Now.Date))
                .Sum(item => item.InvoiceTotal);

            // Calculate today's total payments
            var ToDayTotalPay = suppliers
                .SelectMany(supplier => supplier.ContactPayments
                    .Where(d => d.CreatedAt.Date == DateTime.Now.Date))
                .Sum(payment => payment.Amount);

            // Get last 20 purchases across all suppliers
            var last20Purchases = suppliers
                .SelectMany(supplier => supplier.PurchaseInvoices)
                .Where(d => d.CreatedAt.HasValue) // Ensure CreatedAt is not null
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToList();

            // Get last 20 contact payments across all suppliers
            var Last20ContactPayments = suppliers
                .SelectMany(supplier => supplier.ContactPayments)
                .Where(d => d.CreatedAt.Date == DateTime.Now.Date)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToList();

            // Calculate total purchases from PurchaseInvoices
            var totalPurchase = suppliers
                .SelectMany(supplier => supplier.PurchaseInvoices)
                .Sum(invoice => invoice.InvoiceTotal);

            // Calculate total payments for Outflow
            var totalPay = suppliers
                .SelectMany(supplier => supplier.ContactPayments)
                .Where(payment => payment.PaymentType == PaymentType.Outflow)
                .Sum(payment => payment.Amount);

            // Calculate total supplier debit
            var totalSupplierDebit = totalPurchase - totalPay;

            // Get top 10 suppliers by total purchases
            var top10Suppliers = suppliers
                .Select(supplier => new TopSupplierViewModel
                {
                    Name = supplier.Name,
                    TotalPurchases = supplier.PurchaseInvoices.Sum(invoice => invoice.InvoiceTotal)
                })
                .OrderByDescending(s => s.TotalPurchases)
                .Take(10)
                .ToList();

            // Pass data to view
            var model = new PurchaseDashboardViewModel
            {
                Last20Purchases = last20Purchases,
                TotalSupplierDebit = totalSupplierDebit,
                Top10Suppliers = top10Suppliers,
                ToDayTotalPurchase = ToDayTotalPurchase,
                totalPay = totalPay,
                totalPurchase = totalPurchase,
                Last20ContactPayments = Last20ContactPayments,
                ToDayTotalPay = ToDayTotalPay
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult DeleteInvoice(int id)
        {
            // Find the item by ID
            var item = _context.PurchaseInvoice.Find(id);
            if (item == null)
            {
                return NotFound(); // Return 404 if the item is not found
            }

            // Remove the item
            _context.PurchaseInvoice.Remove(item);
            _context.SaveChanges(); // Save changes to the database

            // Return a success response with a redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("Index") });
        }

        [HttpPost]
        public IActionResult Pay(int purchaseInvoiceId)
        {
            try
            {
                // Step 1: Retrieve the purchase invoice
                var purchaseInvoice = _context.PurchaseInvoices
                    .Include(d => d.PurchaseInvoiceItems)
                    .Include(pi => pi.Contact)
                    .FirstOrDefault(pi => pi.Id == purchaseInvoiceId);

                if (purchaseInvoice == null)
                {
                    return Json(new { success = false, message = "Invoice not found." });
                }

                // Step 2: Retrieve Treasury
                var treasury = _context.Treasuries.FirstOrDefault();
                if (treasury == null)
                {
                    return Json(new { success = false, message = "No Treasury available." });
                }
                var TreasuryId = treasury.TreasuryId;

                // Step 3: Get current logged-in user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Step 4: Calculate total amount for the purchase invoice
                var totalAmount = purchaseInvoice.InvoiceTotal;

                // Log calculated amount for debugging
                Console.WriteLine($"Total Amount for Purchase Invoice ID {purchaseInvoiceId}: {totalAmount}");

                // Step 5: Create the ContactPayment
                var contactPayment = new ContactPayment
                {
                    Amount = totalAmount,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    PaymentType = PaymentType.Outflow,
                    ContactId = purchaseInvoice.Contact.Id,
                    TreasuryId = TreasuryId,
                    UserId = userId,
                    CurrencyId = _DefaultCurrencyId, // Ensure this is properly set
                    Notes = "",
                    PurchaseInvoiceId = purchaseInvoiceId
                };

                // Log ContactPayment details for debugging
                Console.WriteLine($"Creating ContactPayment with TreasuryId {TreasuryId} and Amount {totalAmount}");

                // Step 6: Add payment record to the context and save changes
                _context.ContactPayments.Add(contactPayment);
                _context.SaveChanges();

                // Update PurchaseInvoice's ContactPaymentId
                purchaseInvoice.ContactPaymentId = contactPayment.Id;
                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                // Log database-specific exceptions
                Console.WriteLine("Database Update Exception: " + dbEx.Message);
                Console.WriteLine("Stack Trace: " + dbEx.StackTrace);
                return Json(new { success = false, message = "Database error while marking as paid: " + dbEx.Message });
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine("General Exception in Pay: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                return Json(new { success = false, message = "Error while marking as paid: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult PayByInvoiceItemId(int Id)
        {
            using var transaction = _context.Database.BeginTransaction(); // Start a transaction
            try
            {
                // Step 1: Retrieve the purchase invoice
                var purchaseInvoiceItem = _context.PurchaseInvoiceItems
                    .Include(d => d.PurchaseInvoice)
                    .ThenInclude(d => d.Contact)
                    .FirstOrDefault(d => d.Id == Id);

                if (purchaseInvoiceItem == null)
                {
                    return Json(new { success = false, message = "Invoice not found." });
                }

                // Step 2: Retrieve Treasury
                var treasury = _context.Treasuries.FirstOrDefault();
                if (treasury == null)
                {
                    return Json(new { success = false, message = "No Treasury available." });
                }

                var TreasuryId = treasury.TreasuryId;

                // Step 3: Get current logged-in user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Step 4: Calculate total amount for the purchase invoice
                var totalAmount = purchaseInvoiceItem.LineTotal;

                if (totalAmount <= 0)
                {
                    return Json(new { success = false, message = "Invalid invoice amount." });
                }

                // Step 5: Create the ContactPayment
                var contactPayment = new ContactPayment
                {
                    Amount = totalAmount,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    PaymentType = PaymentType.Outflow,
                    ContactId = purchaseInvoiceItem.PurchaseInvoice.ContactId,
                    TreasuryId = TreasuryId,
                    UserId = userId,
                    CurrencyId = _DefaultCurrencyId, // Ensure this is properly set
                    Notes = string.Empty,
                    PurchaseInvoiceId = purchaseInvoiceItem.PurchaseInvoiceId,
                    PurchaseInvoiceItemId = purchaseInvoiceItem.Id
                };

                _context.ContactPayments.Add(contactPayment);
                _context.SaveChanges();
                // Step 6: Update PurchaseInvoice's ContactPaymentId
                purchaseInvoiceItem.ContactPaymentId = contactPayment.Id;

                // Step 7: Save all changes atomically
                _context.SaveChanges();

                // Commit transaction
                transaction.Commit();

                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                transaction.Rollback(); // Rollback on database-specific exceptions
                Console.WriteLine("Database Update Exception: " + dbEx.Message);
                return Json(new { success = false, message = "Database error while marking as paid: " + dbEx.Message });
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Rollback on general exceptions
                Console.WriteLine("General Exception in Pay: " + ex.Message);
                return Json(new { success = false, message = "Error while marking as paid: " + ex.Message });
            }
        }


        // GET: PurchaseInvoices
        public async Task<IActionResult> InvoiceListDetails(
            int? ContactId,
            DateTime? startDate,
            DateTime? endDate,
            string? Notes,
            bool? IsPaid,
            bool? IsNotPaid,
           int? PurchaseInvoiceId
            )
        {
            // Set default date range to today if not provided
            var currentDate = DateTime.Now.Date;
            startDate ??= currentDate;
            endDate ??= currentDate;

            // Base query with includes
            var query = _context.PurchaseInvoices
                .Where(d => d.BranchId == _BranchId)
                .Include(p => p.Branch)
                .Include(p => p.Contact)
                .Include(p => p.Currency)
                .Include(p => p.User)
                .Include(p => p.PurchaseInvoiceItems)
                .ThenInclude(p => p.Product)
                .AsQueryable();

            // Apply filters
            if (PurchaseInvoiceId.HasValue)
                query = query.Where(e => e.Id == PurchaseInvoiceId);

            if (ContactId.HasValue)
                query = query.Where(e => e.ContactId == ContactId);

            if (!string.IsNullOrEmpty(Notes))
                query = query.Where(e => e.Notes != null && e.Notes.Contains(Notes));

            query = query.Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);

            if (IsPaid.GetValueOrDefault())
                query = query.Where(e => e.PurchaseInvoiceItems.Any(i => i.ContactPaymentId != null));

            if (IsNotPaid.GetValueOrDefault())
                query = query.Where(e => e.PurchaseInvoiceItems.Any(i => i.ContactPaymentId == null));

            // Retrieve contacts for dropdown filter
            ViewBag.Contacts = await _context.Contact
                .Where(d => d.BranchId == _BranchId && d.ContactType == ContactType.Supplier)
                .ToListAsync();
            // Retrieve contacts for dropdown filter
            ViewBag.PurchaseInvoiceId = await _context.PurchaseInvoice
                .Where(d => d.BranchId == _BranchId).Select(d => d.Id)
                .ToListAsync();

            // Execute query
            var filteredInvoices = await query
                 .OrderBy(d => d.CreatedAt)       // First order by CreatedAt
                 .ThenBy(d => d.CurrencyId)       // Then by CurrencyId
                 .ThenBy(d => d.Id)               // Finally, by Id
                 .ToListAsync();


            // Calculate total amount
            ViewBag.TotalAmount = filteredInvoices.Sum(d => d.InvoiceTotal);

            // Pass filter data to ViewBag for the view
            ViewBag.ContactId = ContactId;
            ViewBag.Notes = Notes;
            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;
            ViewBag.IsPaid = IsPaid;
            ViewBag.IsNotPaid = IsNotPaid;

            return View(filteredInvoices);
        }

        // Change this attribute to [HttpGet]
        [HttpGet]
        public IActionResult SaleInvoiceEdit(int id)
        {
            var saleInvoiceItem = _context.SaleInvoiceItem.FirstOrDefault(d => d.Id == id);
            if (saleInvoiceItem == null)
            {
                return NotFound();
            }
            return RedirectToAction("Edit", "SaleInvoices", new { id = saleInvoiceItem.SaleInvoiceId });
        }
    }
}
