using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Purchases;
using ERManager.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]

    public class SaleInvoicesController : Controller
    {
        private readonly ERManagerContext _context;
        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SaleInvoicesController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: SaleInvoices
        public async Task<IActionResult> Index(int? ContactId, DateTime? startDate, DateTime? endDate, string? Notes, bool? IsPaid, bool? IsNotPaid)
        {
            // Default date range to today if no dates are provided
            startDate ??= DateTime.Now.Date;
            endDate ??= DateTime.Now.Date;

            var eRManagerContext = _context.SaleInvoice
                .Where(d => d.BranchId == _BranchId
                && d.CreatedAt.Value.Date >= startDate.Value.Date && d.CreatedAt.Value.Date <= endDate.Value.Date)
                .Include(d => d.SaleInvoiceItems)
               .Include(p => p.Branch)
               .Include(p => p.Contact)
               .Include(p => p.Currency)
               .Include(p => p.User)
               .AsQueryable();

            // Filter by ContactId if provided
            if (ContactId != null)
            {
                eRManagerContext = eRManagerContext.Where(e => e.ContactId == ContactId);
            }

            // Filter by Notes if provided
            if (!string.IsNullOrEmpty(Notes))
            {
                eRManagerContext = eRManagerContext.Where(e => e.Notes != null && e.Notes.Contains(Notes));
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
            ViewBag.Contacts = await _context.Contact.Where(d => d.ContactType == ContactType.Customer && d.BranchId == _BranchId).ToListAsync();

            // Execute the query and get the filtered results
            var filteredInvoices = await eRManagerContext.ToListAsync();

            // Calculate the total amount of filtered invoices
            var totalAmount = filteredInvoices
                .Sum(item => item.InvoiceTotal);

            // Pass data to the ViewBag
            ViewBag.TotalAmount = totalAmount; // Capital 'T' for consistency
            ViewBag.ContactId = ContactId;
            ViewBag.Notes = Notes;
            ViewBag.StartDate = startDate; // Use consistent casing
            ViewBag.EndDate = endDate; // Use consistent casing
            ViewBag.IsPaid = IsPaid;
            ViewBag.IsNotPaid = IsNotPaid;
            return View(filteredInvoices);
        }

        // GET: SaleInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleInvoice = await _context.SaleInvoices.Where(d => d.BranchId == _BranchId)
                .Include(s => s.Branch)
                .Include(s => s.Contact)
                .Include(s => s.Currency)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleInvoice == null)
            {
                return NotFound();
            }

            return View(saleInvoice);
        }

        // GET: SaleInvoices/Create
        public IActionResult Create()
        {
            // Access User here, where it is available
            var logUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if logUserId is null
            if (logUserId == null)
            {
                // Handle the null case (e.g., redirect to login or show an error)
                return RedirectToAction("Login", "Account");
            }
            ViewData["ContactId"] = new SelectList(_context.Contact.Where(d => d.ContactType == ContactType.Customer && d.BranchId == _BranchId), "Id", "Name");
            return View();
        }

        // POST: SaleInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,ContactId")] SaleInvoice saleInvoice)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            saleInvoice.UserId = userId;
            saleInvoice.CurrencyId = _DefaultCurrencyId;
            saleInvoice.BranchId = _BranchId;
            saleInvoice.UpdateAt = DateTime.Now;
            saleInvoice.Tax = 0;
            saleInvoice.Discount = 0;
            // Validate model state

            // Validate model state
            if (!saleInvoice.CreatedAt.HasValue || saleInvoice.ContactId > 0)
            {
                try
                {
                    _context.Add(saleInvoice);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = saleInvoice.Id });
                }
                catch (Exception ex)
                {
                    // Log the error (uncomment ex variable name and log it to your logging framework)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            // If we reach this point, something failed, redisplay form with the data
            ViewData["ContactId"] = new SelectList(_context.Contact.Where(d => d.ContactType == ContactType.Customer && d.BranchId == _BranchId), "Id", "Name", saleInvoice.ContactId);
            return View(saleInvoice); // Return the view to correct any errors

        }

        // GET: SaleInvoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the specific SaleInvoice with necessary includes
            var saleInvoice = await _context.SaleInvoice
                .Where(d => d.BranchId == _BranchId)
                .Include(d => d.Contact)
                .Include(p => p.SaleInvoiceItems)
                    .ThenInclude(item => item.Product) // Only include Product if required for view
                .FirstOrDefaultAsync(m => m.Id == id);

            // Check if saleInvoice was found
            if (saleInvoice == null)
            {
                return NotFound();
            }

            // Fetch all SaleInvoices for the specific contact and branch
            var contactSaleInvoices = await _context.SaleInvoice
                .Where(pi => pi.ContactId == saleInvoice.ContactId && pi.BranchId == _BranchId)
                .AsNoTracking()
                .Include(d => d.SaleInvoiceItems)
                .ToListAsync();

            // Calculate totalInvoiceAmount, defaulting to 0 if no invoices exist for the contact
            var totalInvoiceAmount = contactSaleInvoices.Sum(pi => pi.InvoiceTotal);

            // Calculate total inflow payments for the contact, defaulting to 0 if no payments found
            var totalInflowPayments = await _context.ContactPayments
                .Where(c => c.ContactId == saleInvoice.ContactId && c.PaymentType == PaymentType.Inflow)
                .Select(c => (double?)c.Amount) // Make it nullable to avoid exceptions if no records are found
                .SumAsync() ?? 0;

            // Calculate the balance
            var balance = totalInvoiceAmount - totalInflowPayments;

            // Populate dropdowns for contact and products
            ViewData["ContactId"] = new SelectList(await _context.Contact
                .Where(d => d.ContactType == ContactType.Customer && d.BranchId == _BranchId)
                .ToListAsync(), "Id", "Name", saleInvoice.ContactId);

            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Suppliers = await _context.Contact.Where(d => d.ContactType == ContactType.Supplier).ToListAsync();

            // Pass the SaleInvoice and calculated balance to the view
            ViewBag.Balance = balance;
            ViewBag.InvoiceProfit = saleInvoice.ProfitTotal; // Set to 0 if ProfitTotal is null
            ViewBag.Buyers = new SelectList(await _context.Contact.Where(d => d.IsActive).ToListAsync(), "Id", "Name");

            return View(saleInvoice);
        }


        // POST: SaleInvoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Notes,Discount,Tax,EmployeTax,DriverTax,CreatedAt,ContactId,Expenses")] SaleInvoice saleInvoice)
        {

            // Check if the ID from the route matches the ID in the saleInvoice object
            if (id != saleInvoice.Id)
            {
                return NotFound();
            }

            // Set additional properties
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            saleInvoice.UserId = userId;
            saleInvoice.CurrencyId = _DefaultCurrencyId;
            saleInvoice.BranchId = _BranchId;
            saleInvoice.UpdateAt = DateTime.Now;

            try
            {
                // Update the saleInvoice in the context
                _context.Update(saleInvoice);
                await _context.SaveChangesAsync();
                await CreatePurchaseInvoiceAuto(saleInvoice.Id);
                // Set the success message in TempData
                TempData["SuccessMessage"] = "Invoice updated successfully!";

                // Redirect to the same page (or the desired page)
                return RedirectToAction("Edit", new { id = saleInvoice.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!SaleInvoiceExists(saleInvoice.Id))
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
                return RedirectToAction("Edit", new { id = saleInvoice.Id });
            }
        }


        // GET: SaleInvoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saleInvoice = await _context.SaleInvoices.Where(d => d.BranchId == _BranchId)
                .Include(s => s.Branch)
                .Include(s => s.Contact)
                .Include(s => s.Currency)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saleInvoice == null)
            {
                return NotFound();
            }

            return View(saleInvoice);
        }

        // POST: SaleInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saleInvoice = await _context.SaleInvoices.FindAsync(id);
            if (saleInvoice == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "saleInvoice not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.SaleInvoice.Remove(saleInvoice);
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
                TempData["ErrorMessage"] = "An error occurred while deleting the saleInvoice.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool SaleInvoiceExists(int id)
        {
            return _context.SaleInvoices.Any(e => e.Id == id);
        }


        [HttpPost]
        public IActionResult AddItem(int InvoiceId, int NewProductId, double NewProductQuantity, double NewProductUnitPrice, double NewProductUnitBuyPrice, double NewTaxPresent, int NewContactId)
        {
            // Retrieve the invoice from the database
            var invoice = _context.SaleInvoice.Where(d => d.BranchId == _BranchId).Include(i => i.SaleInvoiceItems)
                                                    .FirstOrDefault(i => i.Id == InvoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            // Create a new item
            var item = new SaleInvoiceItem
            {
                SaleInvoiceId = InvoiceId,
                ProductId = NewProductId,
                Quantity = NewProductQuantity,
                UnitPrice = NewProductUnitPrice,
                UnitBuyPrice = NewProductUnitBuyPrice == 0 ? NewProductUnitPrice : NewProductUnitBuyPrice,
                TaxPresent = NewTaxPresent
              

            };
            if (NewContactId != 0)
                item.ContactId = NewContactId;

            // Add the item to the invoice
            invoice.SaleInvoiceItems.Add(item);

            // Save changes to the database
            _context.SaveChanges();

            // Redirect back to the edit page
            return RedirectToAction("Edit", new { id = InvoiceId });
        }


        [HttpPost]
        public IActionResult EditItem(int id, string field, string value)
        {
            // Fetch the item by id
            var item = _context.SaleInvoiceItem.Find(id);
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
                case "UnitBuyPrice":
                    item.UnitBuyPrice = double.Parse(value);
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
            var item = _context.SaleInvoiceItem.Find(id);
            if (item == null)
            {
                return Json(new { success = false, message = "Item not found." }); // Return a JSON response with an error message
            }

            try
            {
                // Remove the item
                _context.SaleInvoiceItem.Remove(item);
                _context.SaveChanges(); // Save changes to the database

                return Json(new { success = true, message = "Item successfully deleted." }); // Return a success message
            }
            catch (Exception ex)
            {
                // Log the error (optional) and return an error message
                return Json(new { success = false, message = "Error deleting item: " + ex.Message });
            }
        }


        public ActionResult SaleDashboard()
        {
            // Retrieve suppliers with their related PurchaseInvoices and ContactPayments
            var Customers = _context.Contact.Where(d => d.BranchId == _BranchId)
                .Include(c => c.SaleInvoices)
                    .ThenInclude(pi => pi.SaleInvoiceItems)
                .Include(c => c.ContactPayments)
                .Include(c => c.User)
                .Where(d => d.ContactType == ContactType.Customer)
                .ToList(); // Retrieve data in one go
            //
            var ToDayTotalSale = Customers
                .SelectMany(supplier => supplier.SaleInvoices.Where(d => d.CreatedAt.Value.Date == DateTime.Now.Date))
                .Sum(item => item.InvoiceTotal);
            var ToDayTotalPay = Customers
               .SelectMany(supplier => supplier.ContactPayments.Where(d => d.CreatedAt.Date == DateTime.Now.Date))
               .Sum(item => item.Amount);

            // Get last 20 purchases across all suppliers
            var last20Purchases = Customers
                .SelectMany(supplier => supplier.SaleInvoices)
                .Where(d => d.CreatedAt.Value.Date == DateTime.Now.Date)// Flatten the PurchaseInvoices
                .OrderByDescending(p => p.CreatedAt) // Order by CreatedAt
                                                     // Take the last 20 purchases
                .ToList();

            var Last20ContactPayments = Customers
            .SelectMany(supplier => supplier.ContactPayments)
            .Where(d => d.CreatedAt.Date == DateTime.Now.Date)// Flatten the PurchaseInvoices
            .OrderByDescending(p => p.CreatedAt) // Order by CreatedAt
            .ToList();

            // Calculate total purchases from PurchaseInvoices
            var totalPurchase = Customers
                .SelectMany(supplier => supplier.SaleInvoices)
                .Sum(item => item.InvoiceTotal);

            // Calculate total payments for Outflow
            var totalPay = Customers
                .SelectMany(supplier => supplier.ContactPayments)
                .Where(payment => payment.PaymentType == PaymentType.Inflow)
                .Sum(payment => payment.Amount);

            // Calculate total supplier debit
            var totalSupplierDebit = totalPurchase - totalPay;

            // Get top 10 suppliers by total purchases
            var top10Suppliers = Customers
                 .Select(supplier => new TopSupplierViewModel
                 {
                     Name = supplier.Name,
                     TotalPurchases = supplier.SaleInvoices
                         .Sum(item => item.InvoiceTotal)
                 })
                 .OrderByDescending(s => s.TotalPurchases) // Order by total purchases
                 .Take(10) // Take the top 10 suppliers
                 .ToList();



            // Pass data to view
            var model = new SaleDashboardViewModel
            {
                Last20Sales = last20Purchases,
                TotalCustomerDebit = totalSupplierDebit,
                Top10Suppliers = top10Suppliers,
                ToDayTotalPurchase = ToDayTotalSale,
                totalPay = totalPay,
                totalPurchase = totalPurchase,
                Last20ContactPayments = Last20ContactPayments,
                ToDayTotalPay = ToDayTotalPay
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Profit(DateTime? startDate, DateTime? endDate, string? productName, int? categoryId, int? saleInvoiceId, int? contactId)
        {
            // Default date range to today if no dates are provided
            startDate ??= DateTime.Now.Date;
            endDate ??= DateTime.Now.Date;

            // Define the base query with necessary includes for filtering and AsNoTracking for performance
            var query = _context.SaleInvoice
                .Where(d => d.BranchId == _BranchId)
                .AsNoTracking()  // Disable tracking for read-only operations
                .Include(d => d.Contact)
                .Include(d => d.SaleInvoiceItems)
                    .ThenInclude(item => item.Product)
                        .ThenInclude(product => product.ProductCategory)
                .Where(item => 
                            item.CreatedAt.Value.Date >= startDate.Value.Date &&
                            item.CreatedAt.Value.Date <= endDate.Value.Date)
                .AsQueryable();

            // Apply additional filters for product name, category ID, saleInvoiceId, and contactId
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(item => item.SaleInvoiceItems
                                               .Any(d => d.Product.Name.Contains(productName, StringComparison.OrdinalIgnoreCase)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(item => item.SaleInvoiceItems
                                               .Any(d => d.Product.ProductCategoryId == categoryId));
            }

            if (saleInvoiceId.HasValue)
            {
                query = query.Where(item => item.Id == saleInvoiceId);
            }

            if (contactId.HasValue)
            {
                query = query.Where(item => item.ContactId == contactId);
            }

            // Retrieve the filtered data
            var saleInvoices = await query.ToListAsync();

            // Calculate totals (considering each invoice item)
            var totalCost = saleInvoices.Sum(item => item.InvoiceItemsBuyTotal);
            var totalSale = saleInvoices.Sum(item => item.InvoiceItemsTotal);
            var totalDiscount = saleInvoices.Sum(item => item.Discount);
            var totalTax = saleInvoices.Sum(item => item.Tax);
            var totalDriverTax = saleInvoices.Sum(item => item.DriverTax);
            var totalEmployeTax = saleInvoices.Sum(item => item.EmployeTax);
            var totalExpenses = saleInvoices.Sum(item => item.Expenses);

            // Calculate total profit
            var totalProfit = saleInvoices.Sum(item => item.ProfitTotal);

            // Pass the filtered items and totals to the view
            ViewBag.TotalCost = totalCost;
            ViewBag.TotalSale = totalSale;
            ViewBag.TotalProfit = totalProfit;
            ViewBag.TotalDiscount = totalDiscount;
            ViewBag.TotalEmployeTax = totalEmployeTax;
            ViewBag.totalExpenses = totalExpenses;
            ViewBag.TotalDriverTax = totalDriverTax;
            ViewBag.TotalTax = totalTax;
            ViewBag.EndDate = endDate.Value;
            ViewBag.StartDate = startDate.Value;
            ViewBag.ProductName = productName;
            ViewBag.SaleInvoiceId = saleInvoiceId;
            ViewBag.ContactId = contactId;

            // Pass available categories to the view for the dropdown filter
            ViewBag.Categories = new SelectList(await _context.ProductCategories.ToListAsync(), "Id", "Name", categoryId);

            // Pass available contacts to the view for the dropdown filter
            ViewBag.Contact = new SelectList(await _context.Contacts.Where(d => d.ContactType == ContactType.Customer).ToListAsync(), "Id", "Name", contactId);

            return View(saleInvoices);
        }


        public IActionResult Print(int id)
        {
            var saleInvoice = _context.SaleInvoices
                .Include(c => c.Contact)
                .Include(s => s.SaleInvoiceItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(s => s.Id == id);

            if (saleInvoice == null)
            {
                return NotFound();
            }

            return View("Print", saleInvoice);
        }

        [HttpPost]
        public IActionResult DeleteInvoice(int id)
        {
            // Find the item by ID
            var item = _context.SaleInvoice.Find(id);
            if (item == null)
            {
                return NotFound(); // Return 404 if the item is not found
            }

            // Remove the item
            _context.SaleInvoice.Remove(item);
            _context.SaveChanges(); // Save changes to the database

            // Return a success response with a redirect URL
            return Json(new { success = true, redirectUrl = Url.Action("Index") });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CapitalSheet()
        {
            var contacts = await _context.Contact
                .Where(d => d.BranchId == _BranchId)
                .Include(p => p.PurchaseInvoices)
                .ThenInclude(p => p.PurchaseInvoiceItems)
                .Include(s => s.SaleInvoices)
                .ThenInclude(s => s.SaleInvoiceItems)
                .Include(d => d.ContactPayments)
                .ToListAsync();

            var treasuries = await _context.Treasuries
                .Where(d => d.BranchId == _BranchId)
                .Include(d => d.MoneyTransactions)
                .Include(d => d.ContactPayments)
                .Include(d => d.Expenses)
                .Include(d => d.TreasuryMoneyTransfersAsSource)
                .Include(d => d.TreasuryMoneyTransfersAsDestination)
                .ToListAsync();

            var model = new CapitalSheetViewModel
            {
                // Calculating the total debt owed to sellers (Purchase Invoices - Outflow Payments)
                TotalSellerDebt = contacts.Sum(contact =>
                    contact.PurchaseInvoices.Sum(invoice => invoice.InvoiceTotal) -
                    contact.ContactPayments.Where(payment => payment.PaymentType == PaymentType.Outflow).Sum(payment => payment.Amount)
                ),

                // Calculating the total debt owed by buyers (Sale Invoices - Inflow Payments)
                TotalBuyerDebt = contacts.Sum(contact =>
                    contact.SaleInvoices.Sum(invoice => invoice.InvoiceTotal) -
                    contact.ContactPayments.Where(payment => payment.PaymentType == PaymentType.Inflow).Sum(payment => payment.Amount)
                ),

                // Calculating total treasuries balance
                TotalTreasuriesBalance = treasuries.Sum(treasury =>
                    treasury.ContactPayments.Where(payment => payment.PaymentType == PaymentType.Inflow).Sum(payment => payment.Amount) +
                    treasury.MoneyTransactions.Where(transaction => transaction.TransactionType == TransactionType.In).Sum(transaction => transaction.Amount) +
                    treasury.TreasuryMoneyTransfersAsDestination.Sum(transfer => transfer.Amount)
                ) -
                treasuries.Sum(treasury =>
                    treasury.ContactPayments.Where(payment => payment.PaymentType == PaymentType.Outflow).Sum(payment => payment.Amount) +
                    treasury.MoneyTransactions.Where(transaction => transaction.TransactionType == TransactionType.Out).Sum(transaction => transaction.Amount) +
                    treasury.TreasuryMoneyTransfersAsSource.Sum(transfer => transfer.Amount) +
                    treasury.Expenses.Sum(expense => expense.Amount)
                )
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Pay(int saleInvoiceId)
        {
            try
            {
                // Step 1: Retrieve the purchase invoice
                var saleInvoice = _context.SaleInvoice
                    .Include(d => d.SaleInvoiceItems)
                    .Include(pi => pi.Contact)
                    .FirstOrDefault(pi => pi.Id == saleInvoiceId);

                if (saleInvoice == null)
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
                var totalAmount = saleInvoice.InvoiceTotal;

                // Log calculated amount for debugging
                Console.WriteLine($"Total Amount for Purchase Invoice ID {saleInvoiceId}: {totalAmount}");

                // Step 5: Create the ContactPayment
                var contactPayment = new ContactPayment
                {
                    Amount = totalAmount,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    PaymentType = PaymentType.Inflow,
                    ContactId = saleInvoice.Contact.Id,
                    TreasuryId = TreasuryId,
                    UserId = userId,
                    CurrencyId = _DefaultCurrencyId, // Ensure this is properly set
                    Notes = "",
                    SaleInvoiceId = saleInvoiceId
                };

                // Log ContactPayment details for debugging
                Console.WriteLine($"Creating ContactPayment with TreasuryId {TreasuryId} and Amount {totalAmount}");

                // Step 6: Add payment record to the context and save changes
                _context.ContactPayments.Add(contactPayment);
                _context.SaveChanges();

                // Update PurchaseInvoice's ContactPaymentId
                saleInvoice.ContactPaymentId = contactPayment.Id;
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


        // GET: SaleInvoiceItemsBySaleInvoiceId
        public async Task<IActionResult> SaleInvoiceItems(int SaleInvoiceId)
        {
            // Fetch sale invoice items grouped by ContactId
            var groupedItems = await _context.SaleInvoiceItems
                .Where(d => d.SaleInvoiceId == SaleInvoiceId && d.ContactId != null)
                .Include(p => p.Contact)
                .Include(p => p.Product)
                .GroupBy(d => d.ContactId)
                .Select(group => new GroupedSaleInvoiceItemsViewModel
                {
                    ContactId = group.Key,
                    Contact = group.FirstOrDefault().Contact,
                    SaleInvoiceId = group.First().SaleInvoiceId,
                    Items = group.Select(i => new SaleInvoiceItemViewModel
                    {
                        ProductId = i.ProductId,
                        Product = i.Product,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitBuyPrice,
                        IsAddedToPurchase = i.IsAddedToPurchase
                    }).ToList()
                })
                .ToListAsync();

            // Optional: Handle case when no items are found
            if (!groupedItems.Any())
            {
                return NotFound($"No sale invoice items found for SaleInvoiceId: {SaleInvoiceId}");
            }

            // Populate ViewBag if needed
            ViewBag.SaleInvoices = await _context.SaleInvoice.ToListAsync();

            // Return the view with the correctly typed model
            return View(groupedItems);
        }

        public async Task<IActionResult> CreatePurchaseInvoice(int SaleInvoiceId, int ContactId)
        {
            try
            {
                // Retrieve the invoice items from the database
                var saleInvoiceItems = await _context.SaleInvoiceItems
                    .Where(d => d.SaleInvoiceId == SaleInvoiceId && d.ContactId == ContactId && d.IsAddedToPurchase == false)
                    .Include(p => p.Contact)
                    .Include(p => p.Product)
                    .Include(p => p.SaleInvoice) // Include the SaleInvoice entity
                    .ToListAsync();

                if (saleInvoiceItems == null || !saleInvoiceItems.Any())
                {
                    return RedirectToAction(nameof(SaleInvoiceItems), new { SaleInvoiceId });
                }

                var firstItem = saleInvoiceItems.FirstOrDefault();
                if (firstItem?.SaleInvoice == null)
                {
                    return RedirectToAction(nameof(SaleInvoiceItems), new { SaleInvoiceId });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var purchaseInvoice = new PurchaseInvoice
                {
                    BranchId = _BranchId,
                    UserId = userId,
                    ContactId = ContactId,
                    CreatedAt = firstItem.SaleInvoice.CreatedAt,
                    UpdateAt = DateTime.Now,
                    Discount = 0,
                    Tax = 0,
                    CurrencyId = firstItem.SaleInvoice.CurrencyId,
                    InvoiceNumber = "",
                    IsPaid = false,
                    Notes = "",
                    PurchaseInvoiceItems = saleInvoiceItems.Select(i => new PurchaseInvoiceItem
                    {
                        ProductId = i.ProductId,
                        Product = i.Product,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitBuyPrice,
                        TaxPresent=i.TaxPresent,
                        IsPaid=false
                    }).ToList()
                };

                // Update IsAddedToPurchase for all SaleInvoiceItems
                foreach (var item in saleInvoiceItems)
                {
                    item.IsAddedToPurchase = true;
                    _context.SaleInvoiceItems.Update(item);
                }

                _context.PurchaseInvoices.Add(purchaseInvoice);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SaleInvoiceItems), new { SaleInvoiceId });
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism should be set up in your project)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBuyerId(int itemId, int buyerId)
        {
            var item = await _context.SaleInvoiceItems.FindAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            item.ContactId = buyerId; // Update the BuyerId

            try
            {
                await _context.SaveChangesAsync();
                return Ok(); // Success
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public async Task<IActionResult> CreatePurchaseInvoiceAuto(int SaleInvoiceId)
        {
            // Use AsNoTracking to avoid unnecessary entity tracking when data is not modified
            var SaleInvoiceItems = await _context.SaleInvoiceItem
                .Include(d => d.Product)
                .Include(d => d.Contact)
                .Where(d => d.SaleInvoiceId == SaleInvoiceId && d.ContactId != null && d.IsAddedToPurchase == false)
                .ToListAsync();

            // Group the items by ContactId
            var GroupByContactId = SaleInvoiceItems
                .GroupBy(d => d.ContactId)
                .ToList();

            var PurchaseInvoices = new List<PurchaseInvoice>();
            var updatedSaleInvoiceItems = new List<SaleInvoiceItem>();

            foreach (var group in GroupByContactId)
            {
                var purchaseInvoice = new PurchaseInvoice
                {
                    BranchId = _BranchId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ContactId = group.Key ?? 0,
                    CreatedAt = group.First().SaleInvoice.CreatedAt,
                    UpdateAt = DateTime.Now,
                    Discount = 0,
                    Tax = 0,
                    CurrencyId = _DefaultCurrencyId,
                    InvoiceNumber = "", // Consider generating the invoice number dynamically if needed
                    IsPaid = false,
                    Notes = "",
                    PurchaseInvoiceItems = group.Select(i => new PurchaseInvoiceItem
                    {
                        ProductId = i.ProductId,
                        Product = i.Product,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitBuyPrice,
                        TaxPresent = i.TaxPresent,
                        IsPaid=false,
                        
                    }).ToList()
                };

                // Add to list of PurchaseInvoices
                PurchaseInvoices.Add(purchaseInvoice);

                // Mark SaleInvoiceItems as added to Purchase
                foreach (var item in group)
                {
                    item.IsAddedToPurchase = true;
                    updatedSaleInvoiceItems.Add(item);
                }
            }

            // Bulk insert the PurchaseInvoices
            await _context.PurchaseInvoices.AddRangeAsync(PurchaseInvoices);

            // Update SaleInvoiceItems in batch
            _context.SaleInvoiceItem.UpdateRange(updatedSaleInvoiceItems);

            // Save changes in a single transaction
            await _context.SaveChangesAsync();

            // Optionally redirect to another action
            return RedirectToAction(nameof(SaleInvoiceItems), new { SaleInvoiceId });
        }

    }
}
