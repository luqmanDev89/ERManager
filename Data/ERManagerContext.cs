using ERManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERManager.Data
{
    public class ERManagerContext : IdentityDbContext<User>
    {
        public ERManagerContext(DbContextOptions<ERManagerContext> options)
            : base(options)
        {

        }

        // Define DbSet properties for your entities
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Treasury> Treasuries { get; set; } = default!;
        public DbSet<MoneyTransaction> MoneyTransactions { get; set; } = default!;
        public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
        public DbSet<TreasuryMoneyTransfer> TreasuryMoneyTransfers { get; set; } = default!;
        public DbSet<Branch> Branches { get; set; } = default!;
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; } = default!;
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = default!;
        public DbSet<SaleInvoice> SaleInvoices { get; set; } = default!;
        public DbSet<SaleInvoiceItem> SaleInvoiceItems { get; set; } = default!;
        public DbSet<Currency> Currencys { get; set; } = default!;
        public DbSet<Contact> Contacts { get; set; } = default!;
        public DbSet<ExpensesCategory> ExpensesCategorys { get; set; } = default!;
        public DbSet<Expenses> Expensess { get; set; } = default!;
        public DbSet<ContactPayment> ContactPayments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region User relationship
            // User to MoneyTransaction relationship
            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(mt => mt.User)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(mt => mt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Treasury relationship
            modelBuilder.Entity<Treasury>()
                .HasOne(t => t.User)
                .WithMany(u => u.Treasuries)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Contact relationship
            modelBuilder.Entity<Contact>()
                .HasOne(t => t.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to Expenses relationship
            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to TreasuryMoneyTransfer relationship
            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.User)
                .WithMany(u => u.TreasuryMoneyTransfers)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to PurchaseInvoice relationship
            modelBuilder.Entity<PurchaseInvoice>()
                .HasOne(t => t.User)
                .WithMany(u => u.PurchaseInvoices)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to SaleInvoice relationship
            modelBuilder.Entity<SaleInvoice>()
                .HasOne(t => t.User)
                .WithMany(u => u.SaleInvoices)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Treasury Relationship
            modelBuilder.Entity<ContactPayment>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.ContactPayments)
                .HasForeignKey(t => t.TreasuryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.TreasuryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(t => t.TreasuryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.SourceTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsSource)
                .HasForeignKey(t => t.SourceTreasuryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.DestinationTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsDestination)
                .HasForeignKey(t => t.DestinationTreasuryId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Currency Relationship
            modelBuilder.Entity<ContactPayment>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.ContactPayments)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.TreasuryMoneyTransfers)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseInvoice>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.PurchaseInvoices)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleInvoice>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.SaleInvoices)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Branch Relationship
            modelBuilder.Entity<Contact>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Contacts)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Treasury>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Treasuries)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseInvoice>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.PurchaseInvoices)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleInvoice>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.SaleInvoices)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Contact Relationship
            modelBuilder.Entity<Contact>()
                .HasMany(t => t.ContactPayments)
                .WithOne(u => u.Contact)
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasMany(t => t.PurchaseInvoices)
                .WithOne(u => u.Contact)
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasMany(t => t.SaleInvoices)
                .WithOne(u => u.Contact)
                .HasForeignKey(t => t.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Expenses Relationship
            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Category)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Product Relationship
            modelBuilder.Entity<Product>()
                .HasOne(t => t.ProductCategory)
                .WithMany(u => u.Products)
                .HasForeignKey(t => t.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if there are products
            #endregion

            #region Treasury Money Transfer Relationship
            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.SourceTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsSource)
                .HasForeignKey(t => t.SourceTreasuryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if there are transfers

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.DestinationTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsDestination)
                .HasForeignKey(t => t.DestinationTreasuryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if there are transfers
            #endregion

            #region Saleitems Relationship
            // One-to-Many: Contact -> SaleInvoiceItem
            modelBuilder.Entity<SaleInvoiceItem>()
             .HasOne(s => s.Contact)
             .WithMany(c => c.SaleInvoiceItems)
             .HasForeignKey(s => s.ContactId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false); // Allow nulls
            #endregion
        }
        public DbSet<ERManager.Models.Currency> Currency { get; set; } = default!;
        public DbSet<ERManager.Models.Contact> Contact { get; set; } = default!;
        public DbSet<ERManager.Models.ExpensesCategory> ExpensesCategory { get; set; } = default!;
        public DbSet<ERManager.Models.Expenses> Expenses { get; set; } = default!;
        public DbSet<ERManager.Models.ContactPayment> ContactPayment { get; set; } = default!;
        public DbSet<ERManager.Models.PurchaseInvoice> PurchaseInvoice { get; set; } = default!;
        public DbSet<ERManager.Models.PurchaseInvoiceItem> PurchaseInvoiceItem { get; set; } = default!;
        public DbSet<ERManager.Models.SaleInvoice> SaleInvoice { get; set; } = default!;
        public DbSet<ERManager.Models.SaleInvoiceItem> SaleInvoiceItem { get; set; } = default!;

    }
}
