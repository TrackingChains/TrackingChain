using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class AccountProfileGroupBinding
    {
        // Constructors.
        public AccountProfileGroupBinding() { }
        public AccountProfileGroupBinding(AccountProfileGroup accountProfileGroup)
        {
            ArgumentNullException.ThrowIfNull(accountProfileGroup);

            AccountId = accountProfileGroup.AccountId;
            AccountName = accountProfileGroup.Account.Name;
            ProfileGroupId = accountProfileGroup.ProfileGroupId;
            ProfileGroupName = accountProfileGroup.ProfileGroup.Name;
            Priority = accountProfileGroup.Priority;
        }

        // Properties.
        public Guid AccountId { get; set; }
        public string AccountName { get; set; } = default!;
        public Guid ProfileGroupId { get; set; }
        public string ProfileGroupName { get; set; } = default!;
        public int Priority { get; set; }
    }
}
