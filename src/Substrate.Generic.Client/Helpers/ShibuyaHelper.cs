using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types;
using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_core.crypto;
using System;
using System.Linq;

namespace Substrate.Generic.Client.Helpers
{
    public static class ShibuyaHelper
    {
        public static string ToShibuyaHexString(this H256 h256)
        {
            ArgumentNullException.ThrowIfNull(h256);

            return Utils.Bytes2HexString(h256.Value.Value.Select(p => p.Value).ToArray());
        }

        public static byte[] ToShibuyaPublicKey(this string address)
        {
            return Utils.GetPublicKeyFrom(address);
        }

        public static string ToShibuyaAddress(this AccountId32 account32, short ss58 = 42)
        {
            ArgumentNullException.ThrowIfNull(account32);

            var pubKey = account32.Value.Value.Select(p => p.Value).ToArray();
            return pubKey.ToShibuyaAddress(ss58);
        }

        public static string ToShibuyaAddress(this byte[] publicKey, short ss58 = 42)
        {
            return Utils.GetAddressFrom(publicKey, ss58);
        }

        public static AccountId32 ToShibuyaAccountId32(this byte[] publicKey)
        {
            var account32 = new AccountId32();
            account32.Create(publicKey);

            return account32;
        }

        public static AccountId32 ToShibuyaAccountId32(this Account account)
        {
            ArgumentNullException.ThrowIfNull(account);

            var account32 = new AccountId32();
            account32.Create(account.Bytes);

            return account32;
        }

        public static AccountId32 ToShibuyaAccountId32(this string address)
        {
            var account32 = new AccountId32();
            account32.Create(address.ToShibuyaPublicKey());

            return account32;
        }

        public static H256 ToShibuyaH256(this string hash)
        {
            var h256 = new H256();
            h256.Create(hash);
            return h256;
        }
    }
}
