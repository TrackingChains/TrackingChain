using Microsoft.Extensions.Logging;
using Schnorrkel.Keys;
using Substrate.Generic.Client.Helpers;
using Substrate.Generic.Client.Networks;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.ExtraInfos;
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

        // Methods.
        public async Task<string> InsertTrackingAsync(
            string code, 
            string dataValue, 
            string privateKey, 
            int chainNumberId, 
            string chainWs, 
            string contractAddress,
            SubstractContractExtraInfo substractContractExtraInfo,
            CancellationToken token)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(code);
            ArgumentNullException.ThrowIfNullOrEmpty(dataValue);
            ArgumentNullException.ThrowIfNull(substractContractExtraInfo);

            privateKey = "0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a";
            contractAddress = "aKpb5m5WBvTA164EdZhkYHU1SHBixY4QPnxbekMDUSfUYGd";

            var miniSecret = new MiniSecret(Utils.HexToByteArray(privateKey), ExpandMode.Ed25519);
            var account = Account.Build(KeyType.Sr25519, miniSecret.ExpandToSecret().ToBytes(), miniSecret.GetPair().Public.Key);

            //logger.LogInformation("Your address: {address}", Utils.GetAddressFrom(account.Bytes, 5));

            var client = new GenericNetwork(account, chainWs);

            if (!await client.ConnectAsync(true, true, token))
            {
                //logger.LogError("Failed to connect to node");
                return "";
            }

            //logger.LogInformation("Connected to {url}: {flag}", chainWs, client.IsConnected);

            // do action in here ...
            Thread.Sleep(3000);

            var dest = Utils.GetPublicKeyFrom(contractAddress).ToAccountId32();
            var value = new BigInteger(0);
            var refTime = (ulong)3951114240;
            var proofSize = (ulong)125952;
            var storageDepositLimit = new BigInteger(54000000000);
            var data = Utils.HexToByteArray(InputParamsToHex(code, dataValue, false, substractContractExtraInfo));  //"0x1ba63d86363617270650000000000000000000000000000000000000000000000000000014616161616100"
            var subscriptionId = await client.ContractsCallAsync(dest, value, refTime, proofSize, storageDepositLimit, data, 1, token);

            await client.DisconnectAsync();
            //logger.LogInformation("Disconnected from {url}", url);

            if (subscriptionId != null)
            {
                return subscriptionId;
                /*
                logger.LogInformation("SubscriptionId: {subscriptionId}", subscriptionId);
                var queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                while (queueInfo != null && 
                       !queueInfo.IsCompleted)
                {
                    logger.LogInformation("QueueInfo {subscription} [{state}]", subscriptionId, queueInfo != null ? queueInfo.State.ToString() : queueInfo);
                    Thread.Sleep(1000);
                    queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                }*/
            }
            else
            {
                //logger.LogError("Failed to call contract");
                return "";
            }
        }

        // Helpers.
        private string InputParamsToHex(
            string code,
            string dataValue,
            bool closed,
            SubstractContractExtraInfo substractContractExtraInfo)
        {
            var codeHex = code.Replace("0x", "", System.StringComparison.InvariantCultureIgnoreCase);
            var dataValueHex = dataValue.Replace("0x", "", System.StringComparison.InvariantCultureIgnoreCase);
            var prefixDataValueHex = (dataValueHex.Length * 2).ToString("X", CultureInfo.InvariantCulture);
            var closedHex = closed ? "01" : "00";

            return $"{substractContractExtraInfo.InsertTrackSelectorValue}{codeHex}{prefixDataValueHex}{dataValueHex}{closedHex}";
        }
    }
}
