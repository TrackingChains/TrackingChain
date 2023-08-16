using System;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class AccountProfileGroup
    {
        // Constructors.
        public AccountProfileGroup(
            Guid accountId,
            Guid profileGroupId,
            int priority)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException($"{nameof(accountId)} is empty");
            if (profileGroupId == Guid.Empty)
                throw new ArgumentException($"{nameof(profileGroupId)} is empty");

            this.AccountId = accountId;
            this.ProfileGroupId = profileGroupId;
            this.Priority = priority;
        }
        protected AccountProfileGroup() { }

        // Properties.
        public Guid AccountId { get; private set; }
        public Guid ProfileGroupId { get; private set; }
        public int Priority { get; private set; }
        public virtual Account Account { get; set; } = default!;
        public virtual ProfileGroup ProfileGroup { get; set; } = default!;

        // Methods.
        public void Update(int priority)
        {
            Priority = priority;
        }
    }
}
