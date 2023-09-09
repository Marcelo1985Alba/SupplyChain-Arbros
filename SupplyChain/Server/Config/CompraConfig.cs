using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Config;

public class CompraConfig : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        //builder.HasOne(d => d.ProveedorNavigation)
        //       .WithMany(p => p.Compras)
        //       .HasForeignKey(d => d.NROCLTE);
        //.OnDelete(DeleteBehavior.ClientSetNull)
        //.HasConstraintName("FK_Clientes_Companias");
    }
}