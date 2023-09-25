using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.Core.Domain.Entities;
using TrackingChain.TrackingChainCore.EntityFramework;

namespace TrackingChain.Core.EntityFramework.Config
{
    public class ReportSettingConfig : IEntityTypeConfiguration<ReportSetting>
    {
        public void Configure(EntityTypeBuilder<ReportSetting> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Key);

            builder.Property(x => x.Key)
                .HasMaxLength(150);

            builder.ToTable(TableNames.ReportSetting);
        }
    }
}
