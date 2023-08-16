using System;
using TrackingChain.TrackingChainCore.Domain.Entities;

namespace TrackingChain.TriageWebApplication.ModelBinding
{
    public class AccountBinding
    {
        // Constructors.
        public AccountBinding() { }
        public AccountBinding(Account account) 
        {
            ArgumentNullException.ThrowIfNull(account);

            Id = account.Id;
            ChainWatcherAddress = account.ChainWatcherAddress;
            ChainWriterAddress = account.ChainWriterAddress;
            Name = account.Name;
            PrivateKey = account.PrivateKey;
        }

        // Properties.
        public Guid Id { get; set; }
        public string ChainWriterAddress { get; set; } = default!;
        public string ChainWatcherAddress { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string PrivateKey { get; set; } = default!;
        public Guid Profile { get; set; } = default!;
    }
}
