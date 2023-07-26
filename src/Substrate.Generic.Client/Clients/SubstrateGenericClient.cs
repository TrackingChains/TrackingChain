using Microsoft.Extensions.Logging;
using Schnorrkel.Keys;
using Substrate.Generic.Client.Helpers;
using Substrate.Generic.Client.Networks;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Interfaces;

namespace Substrate.Generic.Client.Clients
{
    public class SubstrateGenericClient : ISubstrateClient
    {
        // Fields.
        private readonly ILogger<SubstrateGenericClient> logger;

        // Constractor.
        public SubstrateGenericClient(ILogger<SubstrateGenericClient> logger)
        {
            this.logger = logger;
        }

        public async Task<string> InsertTrackingAsync(string code, string dataValue, string privateKey, int chainNumberId, string chainWs, string contractAddress, CancellationToken token)
        {
            var miniSecret = new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
            var account = Account.Build(KeyType.Sr25519, miniSecret.ExpandToSecret().ToBytes(), miniSecret.GetPair().Public.Key);

            //Log.Information("Your address: {address}", Utils.GetAddressFrom(account.Bytes, 5));

            var client = new GenericNetwork(account, chainWs);

            if (!await client.ConnectAsync(true, true, token))
            {
                //Log.Error("Failed to connect to node");
                return "";
            }

            //Log.Information("Connected to {url}: {flag}", url, client.IsConnected);

            var blockNumber = await client.GetBlocknumberAsync(token);
            //Log.Information("Current block is {number}", blockNumber != null ? blockNumber.Value : null);

            // do action in here ...
            Thread.Sleep(3000);

            var smartContracAddress = "aKpb5m5WBvTA164EdZhkYHU1SHBixY4QPnxbekMDUSfUYGd";
            var dest = Utils.GetPublicKeyFrom(smartContracAddress).ToAccountId32();
            var value = new BigInteger(0);
            var refTime = (ulong)3951114240;
            var proofSize = (ulong)125952;
            var storageDepositLimit = new BigInteger(54000000000);
            var data = Utils.HexToByteArray("0x1ba63d86363617270650000000000000000000000000000000000000000000000000000014616161616100");
            var subscriptionId = await client.ContractsCallAsync(dest, value, refTime, proofSize, storageDepositLimit, data, 1, token);

            await client.DisconnectAsync();

            if (subscriptionId != null)
            {
                return subscriptionId;
                /*
                //Log.Information("SubscriptionId: {subscriptionId}", subscriptionId);
                var queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                while (queueInfo != null && 
                       !queueInfo.IsCompleted)
                {
                    //Log.Information("QueueInfo {subscription} [{state}]", subscriptionId, queueInfo != null ? queueInfo.State.ToString() : queueInfo);
                    Thread.Sleep(1000);
                    queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                }*/
            }
            else
            {
                return "";
                //Log.Error("Failed to call contract");
            }


            
            //Log.Information("Disconnected from {url}", url);
        }
    }
}
