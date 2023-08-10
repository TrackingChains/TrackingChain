using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Substrate.NetApi;
using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.Shibuya.NET.NetApiExt.Generated;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_weights.weight_v2;
using Substrate.Shibuya.NET.NetApiExt.Generated.Storage;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Core.Helpers;

namespace TrackingChain.Core.Clients
{
    public class ShibuyaClient : ISubstrateClient
    {
        // Fields.
        private readonly ChargeType chargeTypeDefault;
        private readonly ILogger<ShibuyaClient> logger;

        // Constructors.
        public ShibuyaClient(
            Account account,
            ILogger<ShibuyaClient> 
            logger, string url)
        {
            Account = account;
            chargeTypeDefault = ChargeTransactionPayment.Default();
            this.logger = logger;

            SubstrateClient = new SubstrateClientExt(new Uri(url), chargeTypeDefault);
        }

        // Properties.
        public Account Account { get; set; }
        public bool IsConnected => SubstrateClient.IsConnected;
        public Substrate.NetApi.SubstrateClient SubstrateClient { get; }

        // Public methods.
        public async Task<bool> ConnectAsync(bool useMetadata, bool standardSubstrate, CancellationToken token)
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
            if (!IsConnected || Account == null)
            {
                return null;
            }

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

            return await GenericExtrinsicAsync(Account, extrinsic, token);
        }

        public async Task<bool> DisconnectAsync()
        {
            if (!IsConnected)
                return false;

            await SubstrateClient.CloseAsync();
            return true;
        }

        internal async Task<string?> GenericExtrinsicAsync(
            Account account,
            Method extrinsicMethod,
            CancellationToken token)
        {
            if (account == null)
                return null;

            if (!IsConnected)
                return null;

            try
            {
                return (await SubstrateClient.Author.SubmitExtrinsicAsync(extrinsicMethod, account, chargeTypeDefault, 64, token))?.Value ?? "";
            }
            catch (RemoteInvocationException ex)
            {
                logger.SubmitExtrinsicError(ex);
                return "";
            }
        }
    }
}
