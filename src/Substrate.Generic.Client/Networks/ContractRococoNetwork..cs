﻿using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Storage;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Core.Clients;
using TrackingChain.Core.Helpers;

namespace TrackingChain.Core.Networks
{
    public class ContractRococoNetwork : ContractRococoBase
    {
        // Constructors.
        public ContractRococoNetwork(Account account, string url) : base(url)
        {
            Account = account;
        }

        // Properties.
        public Account Account { get; set; }

        // Public methods.
        public async Task<string?> ContractsCallAsync(
            AccountId32 dest,
            BigInteger value,
            ulong refTime,
            ulong proofSize,
            BigInteger? storageDepositLimit,
            byte[] data,
            int concurrentTasks,
            bool watchExtrinsic,
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


        public async Task<U32?> GetBlocknumberAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                //Log.Warning("Currently not connected to the network!");
                return null;
            }

            return await SubstrateClient.SystemStorage.Number(token);
        }
    }
}