using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class ProfileGroupConfig : IEntityTypeConfiguration<ProfileGroup>
    {
        public void Configure(EntityTypeBuilder<ProfileGroup> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Accounts)
                   .WithMany(x => x.ProfileGroups)
                   .UsingEntity<AccountProfileGroup>();

            builder.ToTable(TableNames.ProfileGroup);
        }
    }
}
