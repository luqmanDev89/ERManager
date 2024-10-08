using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERManager.Models;

namespace ERManager.Data
{
    public class ERManagerContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region User relationship
            // User to MoneyTransaction relationship
            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(mt => mt.User)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(mt => mt.UserId);

            // User to Treasury relationship
            modelBuilder.Entity<Treasury>()
                .HasOne(t => t.User)
                .WithMany(u => u.Treasuries)
                .HasForeignKey(t => t.UserId);

            // User to Contact relationship
            modelBuilder.Entity<Contact>()
                .HasOne(t => t.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(t => t.UserId);

            // User to ContactPayment relationship
            modelBuilder.Entity<ContactPayment>()
                .HasOne(t => t.User)
                .WithMany(u => u.ContactPayments)
                .HasForeignKey(t => t.UserId);

            // User to Expenses relationship
            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.UserId);

            // User to TreasuryMoneyTransfer relationship
            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.User)
                .WithMany(u => u.TreasuryMoneyTransfers)
                .HasForeignKey(t => t.UserId);
            #endregion

            #region Treasury Relationship
            modelBuilder.Entity<ContactPayment>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.ContactPayments)
                .HasForeignKey(t => t.TreasuryId);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.TreasuryId);

            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(t => t.Treasury)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(t => t.TreasuryId);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.SourceTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsSource) // Renamed
                .HasForeignKey(t => t.SourceTreasuryId);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.DestinationTreasury)
                .WithMany(u => u.TreasuryMoneyTransfersAsDestination) // Renamed
                .HasForeignKey(t => t.DestinationTreasuryId);
            #endregion

            #region Currency RelationShip
            modelBuilder.Entity<ContactPayment>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.ContactPayments)
                .HasForeignKey(t => t.CurrencyId);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CurrencyId);

            modelBuilder.Entity<MoneyTransaction>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.MoneyTransactions)
                .HasForeignKey(t => t.CurrencyId);

            modelBuilder.Entity<TreasuryMoneyTransfer>()
                .HasOne(t => t.Currency)
                .WithMany(u => u.TreasuryMoneyTransfers)
                .HasForeignKey(t => t.CurrencyId);
            #endregion

            #region Branch RelationShip
            modelBuilder.Entity<Contact>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Contacts)
                .HasForeignKey(t => t.BranchId);

            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.BranchId);

            modelBuilder.Entity<Treasury>()
                .HasOne(t => t.Branch)
                .WithMany(u => u.Treasuries)
                .HasForeignKey(t => t.BranchId);
            #endregion

            #region Contact RelationShip
            modelBuilder.Entity<Contact>()
                .HasMany(t => t.ContactPayments)
                .WithOne(u => u.Contact)
                .HasForeignKey(t => t.ContactId);
            #endregion

            #region Expenses RelationShip
            modelBuilder.Entity<Expenses>()
                .HasOne(t => t.Category)
                .WithMany(u => u.Expenses)
                .HasForeignKey(t => t.CategoryId);
            #endregion

            #region Product RelationShip
            modelBuilder.Entity<Product>()
                .HasOne(t => t.ProductCategory)
                .WithMany(u => u.Products)
                .HasForeignKey(t => t.ProductCategoryId);
            #endregion
        }
        public DbSet<ERManager.Models.Currency> Currency { get; set; } = default!;
        public DbSet<ERManager.Models.Contact> Contact { get; set; } = default!;
        public DbSet<ERManager.Models.ExpensesCategory> ExpensesCategory { get; set; } = default!;
        public DbSet<ERManager.Models.Expenses> Expenses { get; set; } = default!;
        public DbSet<ERManager.Models.ContactPayment> ContactPayment { get; set; } = default!;

    }
}
