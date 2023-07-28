using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class TransactionPoolConfig : IEntityTypeConfiguration<TransactionPool>
    {
        public void Configure(EntityTypeBuilder<TransactionPool> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.TrackingId);

            builder.Property(x => x.Code)
                .IsRequired();
            builder.Property(x => x.DataValue)
                .IsRequired();
            builder.Property(x => x.ReceivedDate)
                .IsRequired();
            builder.Property(x => x.Locked)
                .IsConcurrencyToken();

            builder.HasIndex(e => new { e.Locked, e.Priority });

            builder.ToTable(TableNames.TransactionPool);
        }
    }
}
