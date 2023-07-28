using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class AccountProfileGroupConfig : IEntityTypeConfiguration<AccountProfileGroup>
    {
        public void Configure(EntityTypeBuilder<AccountProfileGroup> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => new { x.AccountId, x.ProfileGroupId});
        }
    }
}
