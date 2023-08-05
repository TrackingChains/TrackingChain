﻿using Microsoft.Extensions.Logging;
using Schnorrkel.Keys;
using Substrate.Generic.Client.Helpers;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
using TrackingChain.Core.Clients;
using TrackingChain.Core.Dto;
using TrackingChain.Core.Helpers;

namespace TrackingChain.Core
{
    public class SubstrateClient : IBlockchainService
    {
        // Fields.
        private readonly ILogger<SubstrateClient> logger;

        // Constractor.
        public SubstrateClient(ILogger<SubstrateClient> logger)
        {
            this.logger = logger;
        }

        // Properties.
        public ChainType ProviderType => ChainType.Substrate;

        // Methods.
        public async Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash,
            string chainEndpoint,
            string? apiKey)
        {
            ArgumentNullException.ThrowIfNull(txHash);
            ArgumentNullException.ThrowIfNull(chainEndpoint);

            var url = new Uri($"{chainEndpoint.Trim('/')}/api/scan/extrinsic");

            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

            string jsonData = $"{{\"hash\": \"{txHash}\"}}";
            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var watcherResponseDto = await response.Content.ReadFromJsonAsync<WatcherResponseDto>();

            if (watcherResponseDto?.data is null ||
                !watcherResponseDto.data.finalized ||
                watcherResponseDto.data.pending)
                return null;
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

        public async Task<string> InsertTrackingAsync(
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
                    client = new ContractRococoClient(account, chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToContractRococoAccountId32();
                    break;
                case SupportedClient.Shibuya:
                    client = new ShibuyaClient(account, chainEndpoint);
                    dest = Utils.GetPublicKeyFrom(contractAddress).ToShibuyaAccountId32();
                    break;
                default: throw new NotSupportedException("Client not  supported");
            }


            if (!await client.ConnectAsync(true, true, token))
            {
                //logger.LogError("Failed to connect to node");
                return "";
            }
            //logger.LogInformation("Connected to {url}: {flag}", chainWs, client.IsConnected);

            var insertTrackDto = CreateInsertTrackParams(
                code,
                dataValue,
                false,
                contractExtraInfo);
            var hashTx = await client.ContractsCallAsync(
                dest,
                insertTrackDto.Value,
                insertTrackDto.RefTime,
                insertTrackDto.ProofSize,
                insertTrackDto.StorageDepositLimit,
                insertTrackDto.DataHex,
                token);
            return hashTx ?? "";
        }

        // Helpers.
        private InsertTrackDto CreateInsertTrackParams(
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
            var itemWeight = new List<BigInteger> { new BigInteger(substractContractExtraInfo.BasicWeight) };
            itemWeight.AddRange(Enumerable.Repeat(new BigInteger(substractContractExtraInfo.ByteWeight), dataValueHex.Length / 2));
            var storageDepositLimit = itemWeight.Aggregate((currentSum, item) => currentSum + item);

            return new InsertTrackDto
            {
                DataHex = Utils.HexToByteArray($"{substractContractExtraInfo.InsertTrackSelectorValue}{codeHex}{prefixDataValueHex}{dataValueHex}{closedHex}"),
                ProofSize = substractContractExtraInfo.ProofSize,
                RefTime = substractContractExtraInfo.RefTime,
                StorageDepositLimit = storageDepositLimit,
                Value = new BigInteger(0),
            };
        }
    }
}
