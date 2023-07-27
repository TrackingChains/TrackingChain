using Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_core.crypto;

namespace Substrate.Generic.Client.Helpers
{
    public static class ShibuyaHelper
    {
        public static AccountId32 ToShibuyaAccountId32(this byte[] publicKey)
        {
            var account32 = new AccountId32();
            account32.Create(publicKey);

            return account32;
        }
    }
}
