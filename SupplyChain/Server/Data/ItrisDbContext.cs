using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using SupplyChain.Shared.Itris;

namespace SupplyChain.Server.Data
{
    public class ItrisDbContext : DbContext
    {
        public DbSet<Cotizaciones> Cotizaciones { get; set; }
        public DbSet<vMayorItris> vMayorItris { get; set; }

        public ItrisDbContext(DbContextOptions<ItrisDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(60);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<vMayorItris>().ToView("_ERP_MAYOR");
            base.OnModelCreating(modelBuilder);
        }
    }
}
