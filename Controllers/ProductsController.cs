using ERManager.Data;
using ERManager.Models;
using ERManager.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ProductsController : Controller
    {

        private readonly ERManagerContext _context;

        private readonly int _DefaultCurrencyId = 1;
        private readonly int _BranchId; // Branch ID retrieved from the session
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(ERManagerContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: Products
        public async Task<IActionResult> Index(int? CategoryId, string? Name)
        {
            var eRManagerContext = _context.Products
                .Include(p => p.ProductCategory)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                eRManagerContext = eRManagerContext.Where(d => d.Name.Contains(Name));
            }
            if (CategoryId != null)
            {
                eRManagerContext = eRManagerContext.Where(d => d.ProductCategoryId == CategoryId);
            }
            // Pass categories to populate the Category filter dropdown
            ViewBag.Categories = _context.ProductCategories.ToList();
            ViewBag.CategoryId = CategoryId;
            ViewBag.Name = Name;
            return View(await eRManagerContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.ProductCategories.ToListAsync();

            if (!categories.Any())
            {
                ModelState.AddModelError("", "No categories or treasuries available. Please add them first.");
                return RedirectToAction("Create", "ProductCategories"); // or return View with an error message
            }

            ViewData["categories"] = new SelectList(categories, "Id", "Name");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BuyingPrice,SellingPrice,Barcode,ProductCategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.IsActive = true;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _context.ProductCategories.ToListAsync();

            if (!categories.Any())
            {
                ModelState.AddModelError("", "No categories or treasuries available. Please add them first.");
                return RedirectToAction(nameof(Index)); // or return View with an error message
            }

            ViewData["categories"] = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BuyingPrice,SellingPrice,Barcode,IsActive,ProductCategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategories, "Id", "Id", product.ProductCategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                // If contact is not found, redirect to the Index page with an error message
                TempData["ErrorMessage"] = "product not found or already deleted.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // Optionally set a success message to notify the user
                TempData["SuccessMessage"] = "product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error deleting contact with ID {ContactId}", id);

                // Set an error message to show on the error page
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
                return RedirectToAction("Error", "Home");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Stock(string productName = null, int? categoryId = null)
        {
            // Step 1: Purchase query, grouped by ProductId only
            var purchaseQuery = await _context.PurchaseInvoiceItems
                 .Include(p => p.PurchaseInvoice)
                 .Include(p => p.Product)
                        .ThenInclude(p => p.ProductCategory)
                 .Where(p => p.PurchaseInvoice.BranchId == _BranchId) // Filter by BranchId
                 .GroupBy(p => p.ProductId)
                 .Select(g => new
                 {
                     ProductId = g.Key,
                     ProductName = g.FirstOrDefault().Product.Name ?? "Unknown Product",
                     CategoryId = g.FirstOrDefault().Product.ProductCategoryId, // Ensure this is fetched
                     PurchaseQuantity = g.Sum(p => (double?)p.Quantity) ?? 0 // Default to 0 if null
                 })
                 .ToListAsync();


            // Step 2: Sale query, grouped by ProductId only
            var saleQuery = await _context.SaleInvoiceItems
                .Include(s => s.SaleInvoice)
                .Include(s => s.Product)
                .Where(s => s.SaleInvoice.BranchId == _BranchId)
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    SaleQuantity = g.Sum(s => (double?)s.Quantity) ?? 0 // Default to 0 if null
                })
                .ToListAsync();

            // Step 3: Combine purchase and sale data using a loop
            var stockList = new List<ProductStockViewModel>();

            foreach (var purchase in purchaseQuery)
            {
                var sale = saleQuery.FirstOrDefault(s => s.ProductId == purchase.ProductId);

                stockList.Add(new ProductStockViewModel
                {
                    ProductId = purchase.ProductId,
                    ProductName = purchase.ProductName,
                    PurchaseQuantity = purchase.PurchaseQuantity,
                    SaleQuantity = sale?.SaleQuantity ?? 0, // Safe access for SaleQuantity
                    NetStock = purchase.PurchaseQuantity - (sale?.SaleQuantity ?? 0), // Calculate NetStock safely
                    CategoryId = purchase.CategoryId // Populate CategoryId here
                });
            }

            // Step 4: Filter stockList based on productName and categoryId
            stockList = stockList.Where(s =>
                (string.IsNullOrEmpty(productName) || s.ProductName.ToLower().Contains(productName.ToLower())) &&
                (!categoryId.HasValue || s.CategoryId == categoryId)) // Use CategoryId for filtering
                .ToList();

            // Step 5: Calculate totals
            ViewBag.TotalPurchase = stockList.Sum(d => d.PurchaseQuantity);
            ViewBag.TotalSale = stockList.Sum(d => d.SaleQuantity);
            ViewBag.TotalStock = stockList.Sum(d => d.NetStock);

            // Step 6: Fetch categories for filter dropdown
            ViewBag.categories = await _context.ProductCategories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();

            return View(stockList);
        }


    }
}
