using System;
using TrackingChain.Common.Enums;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class SmartContractBinding
    {
        // Constructors.
        public SmartContractBinding() { }
        public SmartContractBinding(SmartContract smartContract)
        {
            ArgumentNullException.ThrowIfNull(smartContract);

            Id = smartContract.Id;
            Address = smartContract.Address;
            ChainNumberId = smartContract.ChainNumberId;
            ChainType = smartContract.ChainType;
            Currency = smartContract.Currency;
            ExtraInfo = smartContract.ExtraInfo;
            Name = smartContract.Name;
        }

        // Properties.
        public long Id { get; set; }
        public string Address { get; set; } = default!;
        public int ChainNumberId { get; set; }
        public ChainType ChainType { get; set; }
        public string Currency { get; set; } = default!;
        public string ExtraInfo { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
