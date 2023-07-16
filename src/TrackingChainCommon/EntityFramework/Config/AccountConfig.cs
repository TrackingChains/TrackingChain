using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TrackingChainCore.EntityFramework.Config
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.ProfileGroups)
                   .WithMany(x => x.Accounts)
                   .UsingEntity<AccountProfileGroup>();

            builder.ToTable(TableNames.Account);
        }
    }
}
