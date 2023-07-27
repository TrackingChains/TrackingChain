using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace TrackingChain.Core.Helpers
{
    public static class GenericHelper
    {
        public static BigInteger ToPico(double amount) => new(Convert.ToUInt64(amount * Math.Pow(10, 12)));

        public static byte[] ToPublicKey(this string address)
        {
            return Utils.GetPublicKeyFrom(address);
        }

        public static string ToAddress(this byte[] publicKey, short ss58 = 42)
        {
            return Utils.GetAddressFrom(publicKey, ss58);
        }

        public static U8 ToU8(this byte number)
        {
            var u8 = new U8();
            u8.Create(number);
            return u8;
        }
        public static U16 ToU16(this ushort number)
        {
            var u16 = new U16();
            u16.Create(number);
            return u16;
        }

        public static U32 ToU32(this uint number)
        {
            var u32 = new U32();
            u32.Create(number);
            return u32;
        }

        public static U128 ToU128(this BigInteger number)
        {
            var u128 = new U128();
            u128.Create(number);
            return u128;
        }

        public static U8[] ToU8Array(this byte[] bytes)
        {
            return bytes.Select(p => p.ToU8()).ToArray();
        }

        public static U16[] ToU16Array(this ushort[] bytes)
        {
            return bytes.Select(p => p.ToU16()).ToArray();
        }

        public static U32[] ToU32Array(this uint[] bytes)
        {
            return bytes.Select(p => p.ToU32()).ToArray();
        }

        public static U8[] ToU8Array(this string str)
        {
            return str.Select(p => p.ToU8()).ToArray();
        }

        public static U8 ToU8(this char character)
        {
            var u8 = new U8();
            u8.Create(BitConverter.GetBytes(character)[0]);
            return u8;
        }

        public static BaseOpt<U8> ToBaseOpt(this U8 u8)
        {
            var baseOpt = new BaseOpt<U8>();
            baseOpt.Create(u8);
            return baseOpt;
        }

        public static IEnumerable<IEnumerable<T>> BuildChunksWithLinqAndYield<T>(IEnumerable<T> fullList, int batchSize)
        {
            int total = 0;
            while (total < fullList.Count())
            {
                yield return fullList.Skip(total).Take(batchSize);
                total += batchSize;
            }
        }
    }
}
