using Substrate.Generic.Client.Client.Bases;
using Substrate.Generic.Client.Helpers;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.frame_system;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.shibuya_runtime;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_core.crypto;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_weights.weight_v2;
using Substrate.Shibuya.NET.NetApiExt.Generated.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Substrate.Generic.Client.Networks
{
    public class GenericNetwork : BaseClient
    {
        // Constructors.
        public GenericNetwork(Account account, string url) : base(url)
        {
            Account = account;
        }

        // Properties.
        public Account Account { get; set; }

        // Public methods.
        public async Task<string?> BatchAllAsync(
            IEnumerable<EnumRuntimeCall> callList,
            int concurrentTasks,
            CancellationToken token)
        {
            var extrinsicType = "BatchAll";

            if (!IsConnected ||
                Account == null ||
                callList == null ||
                !callList.Any())
            {
                return null;
            }

            var calls = new BaseVec<EnumRuntimeCall>();
            calls.Create(callList.ToArray());

            var extrinsic = UtilityCalls.BatchAll(calls);

            return await GenericExtrinsicAsync(Account, extrinsicType, extrinsic, concurrentTasks, token);
        }

        public async Task<string?> ContractsCallAsync(
            AccountId32 dest,
            BigInteger value,
            ulong refTime,
            ulong proofSize,
            BigInteger? storageDepositLimit,
            byte[] data,
            int concurrentTasks,
            CancellationToken token)
        {
            var extrinsicType = "ContractsCallAsync";

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

            return await GenericExtrinsicAsync(Account, extrinsicType, extrinsic, concurrentTasks, token);
        }

        public async Task<AccountInfo?> GetAccountAsync(CancellationToken token)
        {
            if (Account == null ||
                Account.Value == null)
            {
                //Log.Warning("No account set!");
                return null;
            }

            return await SubstrateClient.SystemStorage.Account(Account.Value.ToAccountId32(), token);
        }

        public async Task<U32?> GetBlocknumberAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                //Log.Warning("Currently not connected to the network!");
                return null;
            }

            return await SubstrateClient.SystemStorage.Number(token);
        }

        public async Task<string?> TransferKeepAliveAsync(
            AccountId32 to, 
            BigInteger amount, 
            int concurrentTasks,
            CancellationToken token)
        {
            var extrinsicType = "TransferKeepAlive";

            if (!IsConnected || Account == null)
            {
                return null;
            }

            var multiAddress = new EnumMultiAddress();
            multiAddress.Create(MultiAddress.Id, to);

            var balance = new BaseCom<U128>();
            balance.Create(amount);

            var extrinsic = BalancesCalls.TransferKeepAlive(multiAddress, balance);

            return await GenericExtrinsicAsync(Account, extrinsicType, extrinsic, concurrentTasks, token);
        }
    }
}
