using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class TransactionRegistryConfig : IEntityTypeConfiguration<TransactionRegistry>
    {
        public void Configure(EntityTypeBuilder<TransactionRegistry> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.TrackingId);

            builder.Property(x => x.Code)
                .IsRequired();
            builder.Property(x => x.DataValue)
                .IsRequired();
            builder.Property(x => x.ReceivedDate)
                .IsRequired();

            builder.ToTable(TableNames.TransactionRegistry);
        }
    }
}
