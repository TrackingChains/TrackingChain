using System;
using System.Text.Json;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.TrackingChainCore.Domain.Enums;

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
            SubstractContractExtraInfo substractContractExtraInfo)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(address);
            ArgumentNullException.ThrowIfNullOrEmpty(name);

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
        public virtual ProfileGroup? ProfileGroup { get; private set; } = default!;
    }
}
