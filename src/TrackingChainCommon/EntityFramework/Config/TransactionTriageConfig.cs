using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class TransactionTriageConfig : IEntityTypeConfiguration<TransactionTriage>
    {
        public void Configure(EntityTypeBuilder<TransactionTriage> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired();
            builder.Property(x => x.Data)
                .IsRequired();
            builder.Property(x => x.ReceivedDate)
                .IsRequired();

            builder.HasIndex(e => new { e.IsInPool, e.Completed });

            builder.ToTable(TableNames.TransactionTriage);
        }
    }
}
