using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using Substrate.NetApi;
using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Substrate.Generic.Client.Helpers;

namespace TrackingChain.Substrate.Generic.Client.Clients
{
    public class BaseClient
    {
        // Fields.
        private readonly ChargeType chargeTypeDefault;
        private readonly ILogger<BaseClient> logger;

        // Constructors.
        public BaseClient(
            Account account,
            ILogger<BaseClient> logger)
        {
            this.Account = account;
            this.chargeTypeDefault = ChargeTransactionPayment.Default();
            this.ExtrinsicManager = new ExtrinsicManager();
            this.logger = logger;
            this.SubscriptionManager = new SubscriptionManager();
        }

        // Properties.
        public Account Account { get; set; }
        public ChargeType ChargeType => chargeTypeDefault;
        public ExtrinsicManager ExtrinsicManager { get; }
        public bool IsConnected => SubstrateClient.IsConnected;
        public SubstrateClient SubstrateClient { get; protected set; } = default!;
        public SubscriptionManager SubscriptionManager { get; }

        // Methods.
        internal async Task<string?> GenericExtrinsicAsync(
            Account account,
            Method extrinsicMethod,
            CancellationToken token)
        {
            if (account == null)
                return null;

            if (!IsConnected)
                return null;

            try
            {
                return (await SubstrateClient.Author.SubmitExtrinsicAsync(extrinsicMethod, account, ChargeType, 64, token))?.Value ?? "";
            }
            catch (RemoteInvocationException ex)
            {
                logger.SubmitExtrinsicError(ex);
                return "";
            }
        }

        internal async Task<string?> GenericExtrinsicAndSubscriptionAsync(
            Account account,
            Method extrinsicMethod,
            string extrinsicType,
            CancellationToken token)
        {
            if (account == null)
                return null;

            if (!IsConnected)
                return null;

            string? subscription = null;
            try
            {
                subscription = await SubstrateClient.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManager.ActionExtrinsicUpdate, extrinsicMethod, account, ChargeType, 64, token);
            }
            catch (RemoteInvocationException ex)
            {
                logger.SubmitExtrinsicError(ex);
                return subscription;
            }

            if (subscription == null)
            {
                return null;
            }

            //Log.Debug("Generic extrinsic sent {0} with {1}.", extrinsicMethod.ModuleName + "_" + extrinsicMethod.CallName, subscription);

            ExtrinsicManager.Add(subscription, extrinsicType);

            return subscription;
        }
    }
}
