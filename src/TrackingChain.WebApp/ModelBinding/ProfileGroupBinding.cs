using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class ProfileGroupBinding
    {
        // Constructors.
        public ProfileGroupBinding() { }
        public ProfileGroupBinding(ProfileGroup profileGroup)
        {
            ArgumentNullException.ThrowIfNull(profileGroup);

            Id = profileGroup.Id;
            AggregationCode = profileGroup.AggregationCode;
            Authority = profileGroup.Authority;
            Category = profileGroup.Category;
            Name = profileGroup.Name;
            SmartContractId = profileGroup.SmartContractId;
            Priority = profileGroup.Priority;
        }

        // Properties.
        public Guid Id { get; private set; }
        public string? AggregationCode { get; set; }
        public string? Authority { get; set; }
        public string? Category { get; set; }
        public string Name { get; set; } = default!;
        public long SmartContractId { get; set; }
        public int Priority { get; set; }
    }
}
