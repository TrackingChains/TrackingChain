using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackingChain.TrackingChainCore.Domain.Entities
{
    public class Account
    {
        // Constructors.
        public Account(
            Guid profile,
            string chainWsAddress,
            string chainRpcAddress,
            string privateKey)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(privateKey);

            if (profile == Guid.Empty)
                throw new ArgumentException($"{nameof(profile)} is empty");

            this.ChainWsAddress = chainWsAddress;
            this.ChainRpcAddress = chainRpcAddress;
            this.PrivateKey = privateKey;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Account() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public string ChainWsAddress { get; protected set; }
        public string ChainRpcAddress { get; protected set; }
        public string PrivateKey { get; private set; }
#pragma warning disable CA1002 // Do not expose generic lists
        public virtual List<ProfileGroup> ProfileGroups { get; } = new();
#pragma warning restore CA1002 // Do not expose generic lists

        public string GetFirstRandomWsAddress
        {
            get
            {
                
                var address = ChainWsAddress.Split(";");
                if (address.Length == 1)
                    return address.First();

                var rnd = new Random();
#pragma warning disable CA5394 // No need secure number
                return address[rnd.Next(address.Length)];
#pragma warning restore CA5394 // No need secure number
            }
        }
        public string GetFirstRandomRpcAddress
        {
            get
            {

                var address = ChainRpcAddress.Split(";");
                if (address.Length == 1)
                    return address.First();

                var rnd = new Random();
#pragma warning disable CA5394 // No need secure number
                return address[rnd.Next(address.Length)];
#pragma warning restore CA5394 // No need secure number
            }
        }

        public IEnumerable<string> GetWsAddress
        {
            get
            {
                return ChainWsAddress.Split(";");
            }
        }

        public IEnumerable<string> GetRpcAddress
        {
            get
            {
                return ChainRpcAddress.Split(";");
            }
        }
    }
}
