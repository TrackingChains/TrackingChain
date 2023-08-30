using System;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class AccountProfileGroup
    {
        // Constructors.
        public AccountProfileGroup(
            string name,
            Guid accountId,
            Guid profileGroupId,
            int priority)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException($"{nameof(accountId)} is empty");
            if (profileGroupId == Guid.Empty)
                throw new ArgumentException($"{nameof(profileGroupId)} is empty");

            this.AccountId = accountId;
            this.Name = name;
            this.ProfileGroupId = profileGroupId;
            this.Priority = priority;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected AccountProfileGroup() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid AccountId { get; private set; }
        public Guid ProfileGroupId { get; private set; }
        public string Name { get; private set; }
        public int Priority { get; private set; }
        public virtual Account Account { get; set; } = default!;
        public virtual ProfileGroup ProfileGroup { get; set; } = default!;

        // Methods.
        public void Update(
            string name,
            int priority)
        {
            Name = name;
            Priority = priority;
        }
    }
}
