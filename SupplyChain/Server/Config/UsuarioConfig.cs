using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Config;

public class UsuarioConfig : IEntityTypeConfiguration<Usuarios>
{
    public void Configure(EntityTypeBuilder<Usuarios> builder)
    {
        throw new NotImplementedException();
    }
}