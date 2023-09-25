using Substrate.NetApi.Model.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackingChain.Substrate.Generic.Client.Clients
{
    public delegate void ExtrinsicFinalizedEvent(string subscriptionId, QueueInfo queueInfo);

    public class ExtrinsicManager
    {
#pragma warning disable CA1003 // Use generic event handler instances
        public event ExtrinsicFinalizedEvent ExtrinsicUpdated;
#pragma warning restore CA1003 // Use generic event handler instances

        public IEnumerable<QueueInfo> Running => _data.Values.Where(p => !p.IsCompleted);

        public IEnumerable<QueueInfo> PreInblock => _data.Values.Where(p => !p.IsInBlock);

        private readonly Dictionary<string, QueueInfo> _data;

        public ExtrinsicManager()
        {
            _data = new Dictionary<string, QueueInfo>();

            ExtrinsicUpdated += OnExtrinsicUpdated;
        }

        public void Add(string subscription, string extrinsicType)
        {
            _data.Add(subscription, new QueueInfo(extrinsicType));
        }

        public QueueInfo? Get(string id)
        {
            if (!_data.TryGetValue(id, out QueueInfo? queueInfo))
            {
                //Log.Debug("QueueInfo not available for subscriptionId {id}", id);
                return null;
            }

            return queueInfo;
        }

        public void ActionExtrinsicUpdate(string subscriptionId, ExtrinsicStatus extrinsicUpdate)
        {
            if (!_data.TryGetValue(subscriptionId, out QueueInfo? queueInfo) || queueInfo == null)
            {
                queueInfo = new QueueInfo("Unknown");
            }

            ArgumentNullException.ThrowIfNull(extrinsicUpdate);
            switch (extrinsicUpdate.ExtrinsicState)
            {
                case ExtrinsicState.None:
                    if (extrinsicUpdate.InBlock?.Value.Length > 0)
                    {
                        queueInfo.Update(QueueInfoState.InBlock, extrinsicUpdate.InBlock.Value);
                        //Log.Debug("Extrinsic {extrinsic} in block {block}", subscriptionId, extrinsicUpdate.InBlock.Value);
                    }
                    else if (extrinsicUpdate.Finalized?.Value.Length > 0)
                    {
                        queueInfo.Update(QueueInfoState.Finalized, extrinsicUpdate.Finalized.Value);
                        _data.Remove(subscriptionId);
                        //Log.Debug("Extrinsic {extrinsic} finalized in block {block}", subscriptionId, extrinsicUpdate.Finalized.Value);
                    }
                    else
                    {
                        queueInfo.Update(QueueInfoState.None);
                        //Log.Debug("Extrinsic {extrinsic} none", subscriptionId);
                    };
                    break;

                case ExtrinsicState.Future:
                    queueInfo.Update(QueueInfoState.Future);
                    //Log.Debug("Extrinsic {extrinsic} future", subscriptionId);
                    break;

                case ExtrinsicState.Ready:
                    queueInfo.Update(QueueInfoState.Ready);
                    //Log.Debug("Extrinsic {extrinsic} ready", subscriptionId);
                    break;

                case ExtrinsicState.Dropped:
                    queueInfo.Update(QueueInfoState.Dropped);
                    _data.Remove(subscriptionId);
                    //Log.Debug("Extrinsic {extrinsic} dropped", subscriptionId);
                    break;

                case ExtrinsicState.Invalid:
                    queueInfo.Update(QueueInfoState.Invalid);
                    _data.Remove(subscriptionId);
                    //Log.Debug("Extrinsic {extrinsic} invalid", subscriptionId);
                    break;
            }

            ExtrinsicUpdated?.Invoke(subscriptionId, queueInfo);
        }

        private void OnExtrinsicUpdated(string subscriptionId, QueueInfo queueInfo)
        {
            /*Log.Debug("{name}[{id}] updated {state}",
                queueInfo.ExtrinsicType,
                subscriptionId,
                queueInfo.State);*/
        }
    }
}
