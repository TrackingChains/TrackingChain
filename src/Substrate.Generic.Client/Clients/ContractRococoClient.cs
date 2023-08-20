using Microsoft.Extensions.Logging;
using Substrate.ContractRococo.NET.NetApiExt.Generated;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Storage;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Substrate.Generic.Client.Helpers;

namespace TrackingChain.Substrate.Generic.Client.Clients
{
    public class ContractRococoClient : BaseClient, ISubstrateClient
    {
        // Fields.
        private readonly ILogger<ContractRococoClient> logger;

        // Constructors.
        public ContractRococoClient(
            Account account,
            ILogger<ContractRococoClient> logger,
            string url)
            : base(account, logger)
        {
            this.SubstrateClient = new SubstrateClientExt(new Uri(url), ChargeType);
            this.logger = logger;
        }

        // Public methods.
        public async Task<bool> ConnectAsync(
            bool useMetadata, 
            bool standardSubstrate, 
            CancellationToken token)
        {
            if (!IsConnected)
                await SubstrateClient.ConnectAsync(useMetadata, standardSubstrate, token);

            return IsConnected;
        }

        public async Task<string?> ContractsCallAsync(
            IType dest,
            BigInteger value,
            ulong refTime,
            ulong proofSize,
            BigInteger? storageDepositLimit,
            byte[] data,
            CancellationToken token)
        {
            if (!IsConnected ||
                Account == null)
                return null;

            return await ExecuteContractsCallAsync(dest, value, refTime, proofSize, storageDepositLimit, data, null, token);
        }

        public async Task<string?> ContractsCallAndWatchAsync(
            IType dest, 
            BigInteger value, 
            ulong refTime, 
            ulong proofSize, 
            BigInteger? storageDepositLimit, 
            byte[] data, 
            string extrinsicType, 
            CancellationToken token)
        {
            ArgumentException.ThrowIfNullOrEmpty(extrinsicType);

            if (!IsConnected ||
                Account == null)
                return null;

            return await ExecuteContractsCallAsync(dest, value, refTime, proofSize, storageDepositLimit, data, extrinsicType, token);
        }

        public async Task<bool> DisconnectAsync()
        {
            if (!IsConnected)
                return false;

            await SubstrateClient.CloseAsync();
            return true;
        }

        // Helpers.
        public async Task<string?> ExecuteContractsCallAsync(
            IType dest,
            BigInteger value,
            ulong refTime,
            ulong proofSize,
            BigInteger? storageDepositLimit,
            byte[] data,
            string? extrinsicType,
            CancellationToken token)
        {
            if (!IsConnected ||
                Account == null)
                return null;

            var destParam = new EnumMultiAddress();
            destParam.Create(MultiAddress.Id, dest);

            var valueParam = new BaseCom<U128>();
            valueParam.Create(value);

            var refTimeParam = new BaseCom<U64>();
            refTimeParam.Create(new CompactInteger(refTime));
            var proofSizeParam = new BaseCom<U64>();
            proofSizeParam.Create(new CompactInteger(proofSize));
            var gasLimitParam = new Weight
            {
                RefTime = refTimeParam,
                ProofSize = proofSizeParam
            };

            var storageDepositLimitParam = new BaseOpt<BaseCom<U128>>();
            if (storageDepositLimit != null)
            {
                var storageDepositLimitValue = new BaseCom<U128>();
                storageDepositLimitValue.Create(storageDepositLimit.Value);
                storageDepositLimitParam.Create(storageDepositLimitValue);
            }

            var dataParam = new BaseVec<U8>();
            dataParam.Create(data.ToU8Array());

            var extrinsic = ContractsCalls.Call(destParam, valueParam, gasLimitParam, storageDepositLimitParam, dataParam);

            if (extrinsicType is null)
                return await GenericExtrinsicAsync(Account, extrinsic, token);
            else
                return await GenericExtrinsicAndSubscriptionAsync(Account, extrinsic, extrinsicType, token);
        }
    }
}
