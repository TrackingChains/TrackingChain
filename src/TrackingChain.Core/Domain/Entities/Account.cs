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
            string chainWriterAddress,
            string chainWatcherAddress,
            string name,
            string privateKey)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(privateKey);

            if (profile == Guid.Empty)
                throw new ArgumentException($"{nameof(profile)} is empty");

            this.ChainWriterAddress = chainWriterAddress;
            this.ChainWatcherAddress = chainWatcherAddress;
            this.Name = name;
            this.PrivateKey = privateKey;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Account() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public Guid Id { get; private set; }
        public string ChainWriterAddress { get; protected set; }
        public string ChainWatcherAddress { get; protected set; }
        public string Name { get; protected set; }
        public string PrivateKey { get; private set; }
#pragma warning disable CA1002 // Do not expose generic lists
        public virtual List<ProfileGroup> ProfileGroups { get; } = new();
#pragma warning restore CA1002 // Do not expose generic lists

        public string GetFirstRandomWriterAddress
        {
            get
            {
                
                var address = ChainWriterAddress.Split(";");
                if (address.Length == 1)
                    return address.First();

                var rnd = new Random();
#pragma warning disable CA5394 // No need secure number
                return address[rnd.Next(address.Length)];
#pragma warning restore CA5394 // No need secure number
            }
        }

        public (string apiUrl, string apiKey) GetFirstRandomWatcherAddress
        {
            get
            {
                var address = ChainWatcherAddress.Split(";");

                var rnd = new Random();
#pragma warning disable CA5394 // No need secure number
                var url = address[rnd.Next(address.Length)];
#pragma warning restore CA5394 // No need secure number

                var splittedUrl = url.Split('|');
                return (splittedUrl[0], splittedUrl.Length == 1 ? "" : splittedUrl[1]);
            }
        }

        public IEnumerable<string> GetWriterAddress
        {
            get
            {
                return ChainWriterAddress.Split(";");
            }
        }

        public IDictionary<string, string> GetWatcherAddress
        {
            get
            {
                Dictionary<string, string> urlToKey = new();
                var urls = ChainWatcherAddress.Split(";");
                foreach (var url in urls)
                {
                    int pipeIndex = url.IndexOf('|', StringComparison.InvariantCultureIgnoreCase);
                    if (pipeIndex >= 0)
                        urlToKey.Add(url[..pipeIndex], url.Substring(pipeIndex + 1, url.Length - pipeIndex - 1));
                    else
                        urlToKey.Add(url, "");
                }

                return urlToKey;
            }
        }
    }
}
