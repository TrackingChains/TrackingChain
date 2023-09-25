using Substrate.NetApi.Model.Rpc;

namespace TrackingChain.Substrate.Generic.Client.Clients
{
    public delegate void SubscriptionOnEvent(string subscriptionId, StorageChangeSet storageChangeSet);

    public class SubscriptionManager
    {
        public bool IsSubscribed { get; set; }

#pragma warning disable CA1003 // Use generic event handler instances
        public event SubscriptionOnEvent SubscrptionEvent;
#pragma warning restore CA1003 // Use generic event handler instances

        public SubscriptionManager()
        {
            SubscrptionEvent += OnSystemEvents;
        }

        public void ActionSubscrptionEvent(string subscriptionId, StorageChangeSet storageChangeSet)
        {
            IsSubscribed = false;

            //Log.Information("System.Events: {0}", storageChangeSet);

            SubscrptionEvent?.Invoke(subscriptionId, storageChangeSet);
        }

        private void OnSystemEvents(string subscriptionId, StorageChangeSet storageChangeSet)
        {
            /*Log.Debug("OnExtrinsicUpdated[{id}] updated {state}",
                subscriptionId,
                storageChangeSet);*/
        }
    }
}
