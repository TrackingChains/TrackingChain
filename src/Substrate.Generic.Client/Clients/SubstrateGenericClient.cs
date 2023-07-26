using Microsoft.Extensions.Logging;
using Schnorrkel.Keys;
using Substrate.Generic.Client.Helpers;
using Substrate.Generic.Client.Networks;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core.Dto;

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

            var dest = Utils.GetPublicKeyFrom(contractAddress).ToAccountId32();
            //var data =   //"0x1ba63d86363617270650000000000000000000000000000000000000000000000000000014616161616100"
            var insertTrackDto = CreateInsertTrackParams(
                code, 
                dataValue, 
                false, 
                substractContractExtraInfo);
            var subscriptionId = await client.ContractsCallAsync(
                dest, 
                insertTrackDto.Value, 
                insertTrackDto.RefTime, 
                insertTrackDto.ProofSize,
                insertTrackDto.StorageDepositLimit,
                insertTrackDto.DataHex, 
                1, 
                false,
                token);
            return subscriptionId ?? "";
            /*
            if (subscriptionId != null)
            {

                //logger.LogInformation("SubscriptionId: {subscriptionId}", subscriptionId);
                var queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                while (queueInfo != null &&
                       !queueInfo.IsCompleted)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA1727 // Use the LoggerMessage delegates
                    logger.LogInformation("QueueInfo {subscription} [{state}]", subscriptionId, queueInfo.State.ToString());
#pragma warning restore CA1727 // Use the LoggerMessage delegates
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                    Thread.Sleep(1000);
                    queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                }
                await client.DisconnectAsync();
                //logger.LogInformation("Disconnected from {url}", url);
                return subscriptionId ?? "";
            }
            else
            {
                //logger.LogError("Failed to call contract");
                return "";
            }
            */
        }

        // Helpers.
        private InsertTrackDto CreateInsertTrackParams(
            string code,
            string dataValue,
            bool closed,
            SubstractContractExtraInfo substractContractExtraInfo)
        {
            // Code.
            var codeHex = Encoding.ASCII.GetBytes(code)
                .Select(b => b.ToString("x2", CultureInfo.InvariantCulture))
                .Aggregate((acc, str) => acc + str)
                .PadRight(64, '0');

            // DataValue.
            var dataValueHex = Encoding.ASCII.GetBytes(dataValue)
                .Select(b => b.ToString("X2", CultureInfo.InvariantCulture))
                .Aggregate((acc, str) => acc + str);
            var prefixDataValueHex = (dataValueHex.Length * 2).ToString("x2", CultureInfo.InvariantCulture);

            // Closed.
            var closedHex = closed ? "01" : "00";

            // Calculate storage deposit limit.
            var itemWeight = new List<BigInteger> { new BigInteger(14000000000) }; // Basic size;
            itemWeight.AddRange(Enumerable.Repeat(new BigInteger(1000000000), dataValueHex.Length / 2)); // Single Hex Size multiplied for number of bytes.
            var storageDepositLimit = itemWeight.Aggregate((currentSum, item) => currentSum + item);

            return new InsertTrackDto
            {
                DataHex = Utils.HexToByteArray($"{substractContractExtraInfo.InsertTrackSelectorValue}{codeHex}{prefixDataValueHex}{dataValueHex}{closedHex}"),
                ProofSize = (ulong)125952,
                RefTime = (ulong)3951114240,
                StorageDepositLimit = storageDepositLimit,
                Value = new BigInteger(0),
            };
        }
    }
}
