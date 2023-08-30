using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class SmartContractConfig : IEntityTypeConfiguration<SmartContract>
    {
        public void Configure(EntityTypeBuilder<SmartContract> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.HasKey(x => x.Id);

            builder.HasMany(sc => sc.ProfileGroups)
                .WithOne(pg => pg.SmartContract)
                .HasForeignKey(pg => pg.SmartContractId);

            builder.ToTable(TableNames.SmartContract);
        }
    }
}
