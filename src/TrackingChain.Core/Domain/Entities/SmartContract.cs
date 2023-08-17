using System;
using System.Collections.Generic;
using System.Text.Json;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class SmartContract
    {
        // Constructors.
        public SmartContract(
            string address, 
            int chainNumberId,
            ChainType chainType,
            string currency,
            string name,
            ContractExtraInfo substractContractExtraInfo)
        {
            ArgumentException.ThrowIfNullOrEmpty(address);
            ArgumentException.ThrowIfNullOrEmpty(name);

            this.Address = address;
            this.ChainNumberId = chainNumberId;
            this.ChainType = chainType;
            this.Currency = currency;
            this.ExtraInfo = JsonSerializer.Serialize(substractContractExtraInfo);
            this.Name = name;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected SmartContract() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public long Id { get; private set; }
        public string Address { get; private set; }
        public int ChainNumberId { get; private set; }
        public ChainType ChainType { get; private set; }
        public string Currency { get; private set; }
        public string ExtraInfo { get; private set; }
        public string Name { get; private set; }
        public virtual ICollection<ProfileGroup> ProfileGroups { get; private set; } = default!;

        // Methods.
        public void Update(
            string address,
            int chainNumberId,
            ChainType chainType,
            string currency,
            string extraInfo,
            string name)
        {
            Address = address;
            ChainNumberId = chainNumberId;
            ChainType = chainType;
            Currency = currency;
            ExtraInfo = extraInfo;
            Name = name;
        }
    }
}
