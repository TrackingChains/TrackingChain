using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;
using TrackingChain.Common.ExtraInfos;
using TrackingChain.Common.Interfaces;

namespace TrackingChain.TransactionRecoveryCore.Services
{
    public class NethereumService : IBlockchainService
    {
        // Fields.
        private readonly ILogger<NethereumService> logger;


        // Constractor.
        public NethereumService(ILogger<NethereumService> logger)
        {
            this.logger = logger;
        }

        // Properties.
        public ChainType ProviderType => ChainType.EVM;

        // Public methods.
        public Task<TransactionDetail?> GetTrasactionDataAsync(
            string code, 
            string contractAddress, 
            string chainEndpoint, 
            int chainNumberId, 
            ContractExtraInfo contractExtraInfo, 
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDetail?> GetTrasactionReceiptAsync(
            string txHash, 
            string chainEndpoint, 
            string? apiKey)
        {
            throw new NotImplementedException();
        }

        public Task<string> InsertTrackingAsync(
            string code, 
            string dataValue, 
            string privateKey, 
            int chainNumberId, 
            string chainEndpoint, 
            string contractAddress, 
            ContractExtraInfo contractExtraInfo, 
            CancellationToken token)
        {
            throw new NotImplementedException();
        }

    }
}
