using System;
using System.Collections.Generic;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class ProfileGroup
    {
        // Constructors.
        public ProfileGroup(
            string? aggregationCode,
            string? authority,
            string? category,
            string name,
            SmartContract smartContract,
            int priority)
        {
            ArgumentNullException.ThrowIfNull(smartContract);
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            this.AggregationCode = aggregationCode;
            this.Authority = authority;
            this.Category = category;
            this.Name = name;
            this.SmartContract = smartContract;
            this.Priority = priority;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ProfileGroup() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public string? AggregationCode { get; private set; }
        public string? Authority { get; private set; }
        public string? Category { get; private set; }
        public string Name { get; private set; }
        public virtual SmartContract SmartContract { get; private set; } = default!;
        public long SmartContractId { get; set; }
        public int Priority { get; private set; }
#pragma warning disable CA1002 // Do not expose generic lists
        public virtual List<Account> Accounts { get; } = new();
#pragma warning restore CA1002 // Do not expose generic lists
    }
}
