using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.primitive_types;
using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using System;
using System.Linq;

namespace TrackingChain.Core.Helpers
{
    public static class ContractRococoHelper
    {
        public static string ToContractRococoHexString(this H256 h256)
        {
            ArgumentNullException.ThrowIfNull(h256);

            return Utils.Bytes2HexString(h256.Value.Value.Select(p => p.Value).ToArray());
        }

        public static byte[] ToContractRococoPublicKey(this string address)
        {
            return Utils.GetPublicKeyFrom(address);
        }

        public static string ToContractRococoAddress(this AccountId32 account32, short ss58 = 42)
        {
            ArgumentNullException.ThrowIfNull(account32);

            var pubKey = account32.Value.Value.Select(p => p.Value).ToArray();
            return pubKey.ToContractRococoAddress(ss58);
        }

        public static string ToContractRococoAddress(this byte[] publicKey, short ss58 = 42)
        {
            return Utils.GetAddressFrom(publicKey, ss58);
        }

        public static AccountId32 ToContractRococoAccountId32(this byte[] publicKey)
        {
            var account32 = new AccountId32();
            account32.Create(publicKey);

            return account32;
        }

        public static AccountId32 ToContractRococoAccountId32(this Account account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var account32 = new AccountId32();
            account32.Create(account.Bytes);

            return account32;
        }

        public static AccountId32 ToContractRococoAccountId32(this string address)
        {
            var account32 = new AccountId32();
            account32.Create(address.ToContractRococoPublicKey());

            return account32;
        }

        public static H256 ToContractRococoH256(this string hash)
        {
            var h256 = new H256();
            h256.Create(hash);
            return h256;
        }
    }
}
