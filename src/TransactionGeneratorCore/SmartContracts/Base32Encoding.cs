using System;
using System.Security.Cryptography;
using System.Text;

namespace TrackingChain.TransactionGeneratorCore.SmartContracts
{
    public static class Base32Encoding
    {
        public static byte[] ToByte32(string inputString)
        {
            var byteArray = Encoding.UTF8.GetBytes(inputString);

            if (byteArray.Length < 32)
            {
                var extendedByteArray = new byte[32];
                Array.Copy(byteArray, extendedByteArray, byteArray.Length);
                byteArray = extendedByteArray;
            }
            else if (byteArray.Length > 32)
                throw new ArgumentException($"Input must byte in 32byte");

            return byteArray;
        }

        public static string FromByte32(byte[] inputBytes32)
        {
            var hashBytes = SHA256.HashData(inputBytes32);
            return Encoding.UTF8.GetString(hashBytes);
        }
    }
}
