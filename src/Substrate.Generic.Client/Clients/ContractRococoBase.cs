using StreamJsonRpc;
using Substrate.ContractRococo.NET.NetApiExt.Generated;
using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrackingChain.Core.Clients
{
    public class ContractRococoBase
    {
        private readonly ChargeType _chargeTypeDefault;

        public SubstrateClientExt SubstrateClient { get; }

        public bool IsConnected => SubstrateClient.IsConnected;

        public ContractRococoBase(string url, int maxConcurrentCalls = 10)
        {
            _chargeTypeDefault = ChargeTransactionPayment.Default();

            SubstrateClient = new SubstrateClientExt(new Uri(url), _chargeTypeDefault);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="useMetadata"></param>
        /// <param name="standardSubstrate"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(bool useMetadata, bool standardSubstrate, CancellationToken token)
        {
            if (!IsConnected)
            {
                try
                {
                    await SubstrateClient.ConnectAsync(useMetadata, standardSubstrate, token);
                }
#pragma warning disable CA1031 // Use generic event handler instances
                catch (Exception)
#pragma warning disable CA1031 // Use generic event handler instances
                {
                    //Log.Error("BaseClient.ConnectAsync: {0}",
                    //e.ToString());
                }
            }

            return IsConnected;
        }

        public async Task<bool> DisconnectAsync()
        {
            if (!IsConnected)
            {
                return false;
            }

            await SubstrateClient.CloseAsync();
            return true;
        }

        internal async Task<string?> GenericExtrinsicAsync(
            Account account,
            Method extrinsicMethod,
            CancellationToken token)
        {
            if (account == null)
            {
                //Log.Warning("Account is null!");
                return null;
            }

            if (!IsConnected)
            {
                //Log.Warning("Currently not connected to the network!");
                return null;
            }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                return (await SubstrateClient.Author.SubmitExtrinsicAsync(extrinsicMethod, account, _chargeTypeDefault, 64, token))?.Value ?? "";
            }
            catch (RemoteInvocationException e)
            {
                //Log.Error("RemoteInvocationException: {0}", e.Message
                //Log.Error("RemoteInvocationException: {0}", e.ErrorData);
                return "";
            }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
    }
}
