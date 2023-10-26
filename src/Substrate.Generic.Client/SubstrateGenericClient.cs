using Microsoft;
using Microsoft.Extensions.Logging;
using Schnorrkel.Keys;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client.Clients;
using TrackingChain.Substrate.Generic.Client.Dto;
using TrackingChain.Substrate.Generic.Client.Helpers;

namespace TrackingChain.Substrate.Generic.Client
{
    public class SubstrateGenericClient : IBlockchainService
    {
        // Fields.
        private readonly ILogger<SubstrateGenericClient> logger;
        private readonly ILoggerFactory loggerFactory;

        // Constractor.
        public SubstrateGenericClient(
            ILogger<SubstrateGenericClient> logger,
            ILoggerFactory loggerFactory)
        {
            this.logger = logger;
            this.loggerFactory = loggerFactory;
        }

        // Properties.
        public ChainType ProviderType => ChainType.Substrate;

        // Methods.
        public async Task<TrackingChainData?> GetTrasactionDataAsync(
            string code,
            string contractAddress,
            string chainEndpoint,
            int chainNumberId,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(contractAddress);
            ArgumentNullException.ThrowIfNull(chainEndpoint);
            ArgumentNullException.ThrowIfNull(contractExtraInfo);

            var miniSecret = new MiniSecret(Utils.HexToByteArray("0x4874841a4694f021ea71a08f5bedd26e6e5f3ecc3240d41d72dad937d20a9d14"), ExpandMode.Ed25519);
            var account = Account.Build(KeyType.Sr25519, miniSecret.ExpandToSecret().ToBytes(), miniSecret.GetPair().Public.Key);

            ISubstrateClient client;
            IType dest;
            switch (contractExtraInfo.SupportedClient)
            {
                case SupportedClient.ContractRococo:
                    client = new ContractRococoClient(account, loggerFactory.CreateLogger<ContractRococoClient>(), chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToContractRococoAccountId32();
                    break;
                case SupportedClient.Shibuya:
                    client = new ShibuyaClient(account, loggerFactory.CreateLogger<ShibuyaClient>(), chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToShibuyaAccountId32();
                    break;
                default:
                    var ex = new NotSupportedException("Client not supported");
                    ex.AddData("Client", contractExtraInfo.SupportedClient);
                    throw ex;
            }

            if (!await client.ConnectAsync(true, true, token))
                return null;
            
            var getTrackingModel = CreateGetTrackingCode(
                code,
                contractExtraInfo);
            var hashTx = await client.ContractsCallAsync(
                false,
                dest,
                getTrackingModel.Value,
                getTrackingModel.RefTime,
                getTrackingModel.ProofSize,
                getTrackingModel.StorageDepositLimit,
                getTrackingModel.DataHex,
                token);

            return null;
            /*
            return new TrackingChainData
            {
                Code = code,
                DataValues = new List<DataDetail> {
                    new DataDetail
                    {
                        BlockNumber = 1,
                        DataValue = Encoding.ASCII.GetBytes("One"),
                        Timestamp = 1234567
                    },
                    new DataDetail
                    {
                        BlockNumber = 2,
                        DataValue = Encoding.ASCII.GetBytes("Two"),
                        Timestamp = 223456711
                    },
                    new DataDetail
                    {
                        BlockNumber = 3,
                        DataValue = Encoding.ASCII.GetBytes("Three"),
                        Timestamp = 323456711
                    }
                  }
            };*/
        }

        public async Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash,
            string chainEndpoint,
            string? apiKey)
        {
            ArgumentNullException.ThrowIfNull(txHash);
            ArgumentNullException.ThrowIfNull(chainEndpoint);

            var url = new Uri($"{chainEndpoint.Trim('/')}/api/scan/extrinsic");

            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            string jsonData = $"{{\"hash\": \"{txHash}\"}}";
            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var watcherResponseDto = await response.Content.ReadFromJsonAsync<WatcherResponseModelView>();

            if (watcherResponseDto?.data is null ||
                !watcherResponseDto.data.finalized ||
                watcherResponseDto.data.pending)
                return null;


            if (!watcherResponseDto.data.success)
                return new TransactionDetail(TransactionErrorReason.TransactionFinalizedInError);
            else
                return new TransactionDetail(
                    watcherResponseDto.data.block_hash,
                    watcherResponseDto.data.block_num.ToString(CultureInfo.InvariantCulture),
                    "",
                    watcherResponseDto.data.fee_used,
                    "",
                    watcherResponseDto.data.error is null ? "" : JsonSerializer.Serialize(watcherResponseDto.data.error),
                    watcherResponseDto.data.account_id,
                    "",
                    watcherResponseDto.data.success,
                    watcherResponseDto.data.extrinsic_hash,
                    "");
        }

        public async Task<TransactionDetail?> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainEndpoint,
            string contractAddress,
            ContractExtraInfo contractExtraInfo,
            CancellationToken token)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(dataValue);
            ArgumentException.ThrowIfNullOrEmpty(privateKey);
            ArgumentException.ThrowIfNullOrEmpty(chainEndpoint);
            ArgumentNullException.ThrowIfNull(contractExtraInfo);

            var miniSecret = new MiniSecret(Utils.HexToByteArray(privateKey), ExpandMode.Ed25519);
            var account = Account.Build(KeyType.Sr25519, miniSecret.ExpandToSecret().ToBytes(), miniSecret.GetPair().Public.Key);

            //logger.LogInformation("Your address: {address}", Utils.GetAddressFrom(account.Bytes, 5));

            ISubstrateClient client;
            IType dest;
            switch (contractExtraInfo.SupportedClient)
            {
                case SupportedClient.ContractRococo:
                    client = new ContractRococoClient(account, loggerFactory.CreateLogger<ContractRococoClient>(), chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToContractRococoAccountId32();
                    break;
                case SupportedClient.Shibuya:
                    client = new ShibuyaClient(account, loggerFactory.CreateLogger<ShibuyaClient>(), chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToShibuyaAccountId32();
                    break;
                default:
                    var ex = new NotSupportedException("Client not supported");
                    ex.AddData("Client", contractExtraInfo.SupportedClient);
                    throw ex;
            }

            if (!await client.ConnectAsync(true, true, token))
                return null;

            var insertTrackDto = CreateInsertTrackParams(
                code,
                dataValue,
                false,
                contractExtraInfo);

            TransactionDetail? transactionDetail = null;
            if (!contractExtraInfo.WaitingForResult)
            {
                var txHash = await client.ContractsCallAsync(
                    true,
                    dest,
                    insertTrackDto.Value,
                    insertTrackDto.RefTime,
                    insertTrackDto.ProofSize,
                    insertTrackDto.StorageDepositLimit,
                    insertTrackDto.DataHex,
                    token) ?? "";
                transactionDetail = new TransactionDetail(txHash);
            }
            else
            {
                var subscriptionId = await client.ContractsCallAndWatchAsync(
                        dest,
                        insertTrackDto.Value,
                        insertTrackDto.RefTime,
                        insertTrackDto.ProofSize,
                        insertTrackDto.StorageDepositLimit,
                        insertTrackDto.DataHex,
                        "InsertTracking",
                        token) ?? "";
                if (!string.IsNullOrWhiteSpace(subscriptionId))
                {
                    //Log.Information("SubscriptionId: {subscriptionId}", subscriptionId);
                    var queueInfo = client.ExtrinsicManager.Get(subscriptionId);
                    while (queueInfo != null && 
                           !queueInfo.IsCompleted)
                    {
#pragma warning disable CA1848
                        logger.LogInformation("Insert OnChain Info {Subscription} [{State}]", subscriptionId, queueInfo.State);
#pragma warning restore CA1848

                        Thread.Sleep(1000);
                        queueInfo = client.ExtrinsicManager.Get(subscriptionId);
                    }
#pragma warning disable CA1848
                    logger.LogInformation("Insert OnChain {Subscription} Finalized", subscriptionId);
#pragma warning restore CA1848
                    transactionDetail = new TransactionDetail(subscriptionId);
                }
                else
                {
#pragma warning disable CA1848
                    logger.LogError("Insert OnChain Error");
#pragma warning restore CA1848
                }
            }
            await client.DisconnectAsync();

            return transactionDetail;
        }

        // Helpers.
        private TrackingChainCallerModel CreateInsertTrackParams(
            string code,
            string dataValue,
            bool closed,
            ContractExtraInfo substractContractExtraInfo)
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
            var itemWeight = new List<BigInteger> { new BigInteger(substractContractExtraInfo.InsertTrackBasicWeight) };
            itemWeight.AddRange(Enumerable.Repeat(new BigInteger(substractContractExtraInfo.ByteWeight), dataValueHex.Length / 2));
            var storageDepositLimit = itemWeight.Aggregate((currentSum, item) => currentSum + item);

            return new TrackingChainCallerModel
            {
                DataHex = Utils.HexToByteArray($"{substractContractExtraInfo.InsertTrackSelectorValue}{codeHex}{prefixDataValueHex}{dataValueHex}{closedHex}"),
                ProofSize = substractContractExtraInfo.InsertTrackProofSize,
                RefTime = substractContractExtraInfo.InsertTrackRefTime,
                StorageDepositLimit = storageDepositLimit,
                Value = new BigInteger(0),
            };
        }

        private TrackingChainCallerModel CreateGetTrackingCode(
            string code,
            ContractExtraInfo substractContractExtraInfo)
        {
            // Code.
            var codeHex = Encoding.ASCII.GetBytes(code)
                .Select(b => b.ToString("x2", CultureInfo.InvariantCulture))
                .Aggregate((acc, str) => acc + str)
                .PadRight(64, '0');

            // Calculate storage deposit limit.
            var storageDepositLimit = new BigInteger(substractContractExtraInfo.InsertTrackBasicWeight);

            return new TrackingChainCallerModel
            {
                DataHex = Utils.HexToByteArray($"{substractContractExtraInfo.GetTrackSelectorValue}{codeHex}"),
                ProofSize = 1000000,
                RefTime = 1000000,
                StorageDepositLimit = 0,
                Value = new BigInteger(0),
            };
        }
    }
}
