using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Config
{
    public class PedidoConfig : IEntityTypeConfiguration<Pedidos>
    {
        public void Configure(EntityTypeBuilder<Pedidos> builder)
        {
            builder.HasOne(d => d.Proveedor)
                   .WithMany(p => p.Stocks)
                   .HasPrincipalKey(p => new { p.CG_PROVE })
                   .HasForeignKey(d => d.CG_PROVE)
                   .OnDelete(DeleteBehavior.NoAction)
                   //.HasConstraintName("FK_Pedidos_Prove1")
                   .IsRequired(false);

            //.OnDelete(DeleteBehavior.ClientSetNull)
            //.HasConstraintName("FK_Clientes_Companias");

        }
    }
}
