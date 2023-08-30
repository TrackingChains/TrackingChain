using Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto;

namespace TrackingChain.Substrate.Generic.Client.Helpers
{
    public static class ContractRococoHelper
    {
        public static AccountId32 ToContractRococoAccountId32(this byte[] publicKey)
        {
            var account32 = new AccountId32();
            account32.Create(publicKey);

            return account32;
        }
    }
}
