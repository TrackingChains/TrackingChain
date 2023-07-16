using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class TransactionPendingConfig : IEntityTypeConfiguration<TransactionPending>
    {
        public void Configure(EntityTypeBuilder<TransactionPending> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.TrackingId);

            builder.Property(x => x.Code)
                .IsRequired();
            builder.Property(x => x.Data)
                .IsRequired();
            builder.Property(x => x.Locked)
                .IsConcurrencyToken();
            builder.Property(x => x.ReceivedDate)
                .IsRequired();

            builder.ToTable(TableNames.TransactionPending);
        }
    }
}
