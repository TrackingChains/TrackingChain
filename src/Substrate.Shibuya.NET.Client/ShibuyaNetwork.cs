using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Shibuya.Integration.Client;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.shibuya_runtime;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.frame_system;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_core.crypto;

using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Substrate.Shibuya.NET.NetApiExt.Generated.Storage;
using Serilog;
using Shibuya.Integration.Helper;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_weights.weight_v2;

namespace Shibuya.Integration
{
    public partial class ShibuyaNetwork : BaseClient
    {
        public Account Account { get; set; }

        public ShibuyaNetwork(Account account, string url) : base(url)
        {
            Account = account;
        }

        /// <summary>
        /// Get the current block number
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<U32?> GetBlocknumberAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                Log.Warning("Currently not connected to the network!");
                return null;
            }

            return await SubstrateClient.SystemStorage.Number(token);
        }

        /// <summary>
        /// Get the account infos for the current account
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<AccountInfo> GetAccountAsync(CancellationToken token)
        {
            if (Account == null || Account.Value == null)
            {
                Log.Warning("No account set!");
                return null;
            }

            return await SubstrateClient.SystemStorage.Account(Account.Value.ToAccountId32(), token);
        }

        /// <summary>
        /// Execute a transfer keep alive extrinsic
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string?> TransferKeepAliveAsync(AccountId32 to, BigInteger amount, int concurrentTasks, CancellationToken token)
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

        /// <summary>
        /// Contracts Call extrinsic
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="value"></param>
        /// <param name="gasLimit"></param>
        /// <param name="storageDepositLimit"></param>
        /// <param name="data"></param>
        /// <param name="concurrentTasks"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string?> ContractsCallAsync(AccountId32 dest, BigInteger value, ulong refTime, ulong proofSize, BigInteger? storageDepositLimit, byte[] data, int concurrentTasks, CancellationToken token)
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
            var gasLimitParam = new Weight();
            gasLimitParam.RefTime = refTimeParam;
            gasLimitParam.ProofSize = proofSizeParam;

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

        /// <summary>
        /// Execute a a bath of extrinsics
        /// </summary>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string?> BatchAllAsync(List<EnumRuntimeCall> callList, int concurrentTasks, CancellationToken token)
        {
            var extrinsicType = "BatchAll";

            if (!IsConnected || Account == null || callList == null || callList.Count == 0)
            {
                return null;
            }

            var calls = new BaseVec<EnumRuntimeCall>();
            calls.Create(callList.ToArray());

            var extrinsic = UtilityCalls.BatchAll(calls);

            return await GenericExtrinsicAsync(Account, extrinsicType, extrinsic, concurrentTasks, token);
        }

    }
}