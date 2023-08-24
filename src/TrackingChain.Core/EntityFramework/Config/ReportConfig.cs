using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework;

namespace TrackingChain.Core.EntityFramework.Config
{
    public class ReportConfig : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Id);

            builder.ToTable(TableNames.Report);
        }
    }
}
