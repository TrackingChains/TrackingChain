using Schnorrkel.Keys;
using StreamJsonRpc;
using Substrate.Astar.NET.NetApiExt.Generated;
using Substrate.Astar.NET.NetApiExt.Generated.Storage;
using Substrate.NetApi;
using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrackingChain.TransactionGeneratorCore.SubstrateClient
{
    public class SubstrateBaseClient
    {
        private readonly int _maxConcurrentCalls;

        private readonly ChargeType _chargeTypeDefault;

        public static MiniSecret MiniSecretAlice => new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
        public static Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(), MiniSecretAlice.GetPair().Public.Key);

        public ExtrinsicManager ExtrinsicManger { get; }

        public SubscriptionManager SubscriptionManager { get; }

        public SubstrateClientExt SubstrateClient { get; }

        public bool IsConnected => SubstrateClient.IsConnected;

        public SubstrateBaseClient(string url, int maxConcurrentCalls = 10)
        {
            _chargeTypeDefault = ChargeTransactionPayment.Default();
            //_chargeTypeDefault = ChargeAssetTxPayment.Default();

            _maxConcurrentCalls = maxConcurrentCalls;

            SubstrateClient = new SubstrateClientExt(new Uri(url), _chargeTypeDefault);

            ExtrinsicManger = new ExtrinsicManager();

            SubscriptionManager = new SubscriptionManager();
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
#pragma warning disable CA1031 // Unnecessary assignment of a value
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
                try
                {
                    await SubstrateClient.ConnectAsync(useMetadata, standardSubstrate, token);
                }
                catch (Exception e)
                {
                    /*Log.Error("BaseClient.ConnectAsync: {0}",
                        e.ToString());*/
                }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CA1031 // Unnecessary assignment of a value
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="account"></param>
        /// <param name="extrinsicType"></param>
        /// <param name="extrinsicMethod"></param>
        /// <param name="concurrentTasks"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string?> GenericExtrinsicAsync(Account account, string extrinsicType, Method extrinsicMethod, int concurrentTasks, CancellationToken token)
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

            if (ExtrinsicManger.Running.Count() >= _maxConcurrentCalls)
            {
                //Log.Warning("There can not be more then {0} concurrent tasks overall!", _maxConcurrentCalls);
                return null;
            }

            if (ExtrinsicManger.Running.Count(p => p.ExtrinsicType == extrinsicType) >= concurrentTasks)
            {
                //Log.Warning("There can not be more then {0} concurrent tasks of {1}!", concurrentTasks, extrinsicType);
                return null;
            }

            string? subscription = null;
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                subscription = await SubstrateClient.Author.SubmitAndWatchExtrinsicAsync(ExtrinsicManger.ActionExtrinsicUpdate, extrinsicMethod, account, _chargeTypeDefault, 64, token);
            }
            catch (RemoteInvocationException e)
            {
                //Log.Error("RemoteInvocationException: {0}", e.Message);
                return subscription;
            }
#pragma warning restore CS0168 // Variable is declared but never used

            if (subscription == null)
            {
                return null;
            }

            //Log.Debug("Generic extrinsic sent {0} with {1}.", extrinsicMethod.ModuleName + "_" + extrinsicMethod.CallName, subscription);

            ExtrinsicManger.Add(subscription, extrinsicType);
            return subscription;
        }

        public async Task<string?> SubscribeEventsAsync(CancellationToken token)
        {
            if (!IsConnected)
            {
                //Log.Warning("Currently not connected to the network!");
                return null;
            }

            if (SubscriptionManager.IsSubscribed)
            {
                //Log.Warning("Already active subscription to events!");
                return null;
            }

            var subscription = await SubstrateClient.SubscribeStorageKeyAsync(SystemStorage.EventsParams(), SubscriptionManager.ActionSubscrptionEvent, token);
            if (subscription == null)
            {
                return null;
            }

            SubscriptionManager.IsSubscribed = true;

            //Log.Debug("SystemStorage.Events subscription id {0] registred.", subscription);

            return subscription;
        }

        public static Account RandomAccount(int seed, string derivationPsw = "aA1234dd", KeyType keyType = KeyType.Sr25519)
        {
            var random = new Random(seed);
            var randomBytes = new byte[16];
#pragma warning disable CA5394 // Do not use insecure randomness
            random.NextBytes(randomBytes);
#pragma warning restore CA5394 // Do not use insecure randomness
            var mnemonic = string.Join(" ", Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
            //Log.Information("mnemonic[Sr25519]: {0}", mnemonic);
            return Mnemonic.GetAccountFromMnemonic(mnemonic, derivationPsw, keyType);
        }
    }
}
