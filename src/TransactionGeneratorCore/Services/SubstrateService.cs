﻿using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Substrate.Astar.NET.NetApiExt.Generated;
using Substrate.NetApi.Model.Extrinsics;
using System;
using System.Text;
using System.Threading.Tasks;
using TrackingChain.TransactionGeneratorCore.SmartContracts;

namespace TrackingChain.TransactionGeneratorCore.Services
{
    public class SubstrateService : IEthereumService
    {
        // Fields.
        private readonly ILogger<SubstrateService> logger;
        private readonly INodeJSService nodeJSService;

        // Constractor.
        public SubstrateService(
            ILogger<SubstrateService> logger,
            INodeJSService nodeJSService)
        {
            this.logger = logger;
            this.nodeJSService = nodeJSService;
        }

        // Public methods.
        public async Task<string> InsertTrackingAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress)
        {
            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainRpc,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            return await contractHandler.SendRequestAsync(insertTracking);
        }

        // Methods.
        public async Task<TransactionReceipt> InsertTrackingAndWaitForReceiptAsync(
            string code,
            string dataValue,
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress)
        {
            ResultSubstrateCall? result = await nodeJSService.InvokeFromFileAsync<ResultSubstrateCall>("exampleModule.js", args: new[] { "success" });
            

            var contractHandler = GetContractHandler(
                privateKey,
                chainNumberId,
                chainRpc,
                contractAddress);

            var insertTracking = CommonInsertTracking(
                code,
                dataValue);

            return await contractHandler.SendRequestAndWaitForReceiptAsync(insertTracking);
        }

        // Helpers.
        private InsertTrackFunction CommonInsertTracking(
            string code,
            string dataValue)
        {
            return new InsertTrackFunction
            {
                Code = Encoding.ASCII.GetBytes(code),
                DataValue = Encoding.ASCII.GetBytes(dataValue),
                Closed = false
            };
        }

        private ContractHandler GetContractHandler(
            string privateKey,
            int chainNumberId,
            string chainRpc,
            string contractAddress)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey, chainNumberId);
            var web3 = new Web3(account, chainRpc);

            return web3.Eth.GetContractHandler(contractAddress);
        }

        public class ResultSubstrateCall
        {

        }
    }
}
