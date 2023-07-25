using Substrate.NetApi.Model.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingChain.TransactionGeneratorCore.SubstrateClient
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="extrinsicType"></param>
        public void Add(string subscription, string extrinsicType)
        {
            _data.Add(subscription, new QueueInfo(extrinsicType));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QueueInfo? Get(string id)
        {
            if (!_data.TryGetValue(id, out QueueInfo? queueInfo))
            {
                //Log.Debug("QueueInfo not available for subscriptionId {id}", id);
                return null;
            }

            return queueInfo;
        }

        /// <summary>
        /// Simple extrinsic tester
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="extrinsicUpdate"></param>
        public void ActionExtrinsicUpdate(string subscriptionId, ExtrinsicStatus extrinsicUpdate)
        {
            if (extrinsicUpdate is null)
                return;

            if (!_data.TryGetValue(subscriptionId, out QueueInfo? queueInfo) || queueInfo == null)
            {
                queueInfo = new QueueInfo("Unknown");
            }

            switch (extrinsicUpdate.ExtrinsicState)
            {
                case ExtrinsicState.None:
                    if (extrinsicUpdate.InBlock?.Value.Length > 0)
                    {
                        //var inBlock = extrinsicUpdate.InBlock.Value.Substring(0, 10);
                        queueInfo.Update(QueueInfoState.InBlock);
                    }
                    else if (extrinsicUpdate.Finalized?.Value.Length > 0)
                    {
                        //var finalized = extrinsicUpdate.Finalized.Value.Substring(0, 10);
                        queueInfo.Update(QueueInfoState.Finalized);
                        _data.Remove(subscriptionId);
                    }
                    else
                    {
                        queueInfo.Update(QueueInfoState.None);
                    };
                    break;

                case ExtrinsicState.Future:
                    queueInfo.Update(QueueInfoState.Future);
                    break;

                case ExtrinsicState.Ready:
                    queueInfo.Update(QueueInfoState.Ready);
                    break;

                case ExtrinsicState.Dropped:
                    queueInfo.Update(QueueInfoState.Dropped);
                    _data.Remove(subscriptionId);
                    break;

                case ExtrinsicState.Invalid:
                    queueInfo.Update(QueueInfoState.Invalid);
                    _data.Remove(subscriptionId);
                    break;
            }

            ExtrinsicUpdated?.Invoke(subscriptionId, queueInfo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="queueInfo"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnExtrinsicUpdated(string subscriptionId, QueueInfo queueInfo)
        {
            /*Log.Debug("{name}[{id}] updated {state}",
                queueInfo.ExtrinsicType,
                subscriptionId,
                queueInfo.State);*/
        }
    }
}
