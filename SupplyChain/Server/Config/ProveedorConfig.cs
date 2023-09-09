using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Config;

public class ProveedorConfig : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        //builder.HasMany(p => p.Stocks)
        //    .WithOne(p => p.Proveedor)
        //    .HasPrincipalKey(p => new { p.CG_PROVE })
        //    .IsRequired(false);
        //.HasForeignKey(d => d.CG_PROVE);
    }
}