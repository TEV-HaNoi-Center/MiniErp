using Microsoft.EntityFrameworkCore;
using MiniErp.Domain;
using MiniErp.Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.EFCore
{
    public class MiniErpContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryNote> DeliveryNotes { get; set; }
        public DbSet<DeliveryNoteDetail> DeliveryNotesDetail { get; set; } 
        public DbSet<Product> Products { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ReceiveNote> ReceiveNotes { get; set; }
        public DbSet<ReceiveNoteDetail> ReceiveNotesDetail { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TimeKeeping> TimeKeepings { get; set; }

        public MiniErpContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryNoteConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryNoteDetailConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ReceiveNoteConfiguration());
            modelBuilder.ApplyConfiguration(new ReceiveNoteDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TimeKeepingConfiguration());
        }
    }
}
