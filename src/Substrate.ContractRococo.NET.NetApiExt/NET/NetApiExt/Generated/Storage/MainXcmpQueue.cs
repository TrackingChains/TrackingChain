//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Substrate.NetApi;
using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Meta;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Base;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Substrate.ContractRococo.NET.NetApiExt.Generated.Storage
{
    
    
    public sealed class XcmpQueueStorage
    {
        
        // Substrate client for the storage calls.
        private SubstrateClientExt _client;
        
        public XcmpQueueStorage(SubstrateClientExt client)
        {
            this._client = client;
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "InboundXcmpStatus"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.InboundChannelDetails>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "InboundXcmpMessages"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                            Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat,
                            Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, typeof(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32>), typeof(Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "OutboundXcmpStatus"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.OutboundChannelDetails>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "OutboundXcmpMessages"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                            Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat,
                            Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, typeof(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U16>), typeof(Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "SignalMessages"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                            Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat}, typeof(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id), typeof(Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "QueueConfig"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.QueueConfigData)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "Overweight"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                            Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, typeof(Substrate.NetApi.Model.Types.Primitive.U64), typeof(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "CounterForOverweight"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.NetApi.Model.Types.Primitive.U32)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "OverweightCount"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.NetApi.Model.Types.Primitive.U64)));
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("XcmpQueue", "QueueSuspended"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.NetApi.Model.Types.Primitive.Bool)));
        }
        
        /// <summary>
        /// >> InboundXcmpStatusParams
        ///  Status of the inbound XCMP channels.
        /// </summary>
        public static string InboundXcmpStatusParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "InboundXcmpStatus", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> InboundXcmpStatusDefault
        /// Default value as hex string
        /// </summary>
        public static string InboundXcmpStatusDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> InboundXcmpStatus
        ///  Status of the inbound XCMP channels.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.InboundChannelDetails>> InboundXcmpStatus(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.InboundXcmpStatusParams();
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.InboundChannelDetails>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> InboundXcmpMessagesParams
        ///  Inbound aggregate XCMP messages. It can only be one per ParaId/block.
        /// </summary>
        public static string InboundXcmpMessagesParams(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32> key)
        {
            return RequestGenerator.GetStorage("XcmpQueue", "InboundXcmpMessages", Substrate.NetApi.Model.Meta.Storage.Type.Map, new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                        Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat,
                        Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, key.Value);
        }
        
        /// <summary>
        /// >> InboundXcmpMessagesDefault
        /// Default value as hex string
        /// </summary>
        public static string InboundXcmpMessagesDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> InboundXcmpMessages
        ///  Inbound aggregate XCMP messages. It can only be one per ParaId/block.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>> InboundXcmpMessages(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32> key, CancellationToken token)
        {
            string parameters = XcmpQueueStorage.InboundXcmpMessagesParams(key);
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> OutboundXcmpStatusParams
        ///  The non-empty XCMP channels in order of becoming non-empty, and the index of the first
        ///  and last outbound message. If the two indices are equal, then it indicates an empty
        ///  queue and there must be a non-`Ok` `OutboundStatus`. We assume queues grow no greater
        ///  than 65535 items. Queue indices for normal messages begin at one; zero is reserved in
        ///  case of the need to send a high-priority signal message this block.
        ///  The bool is true if there is a signal message waiting to be sent.
        /// </summary>
        public static string OutboundXcmpStatusParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "OutboundXcmpStatus", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> OutboundXcmpStatusDefault
        /// Default value as hex string
        /// </summary>
        public static string OutboundXcmpStatusDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> OutboundXcmpStatus
        ///  The non-empty XCMP channels in order of becoming non-empty, and the index of the first
        ///  and last outbound message. If the two indices are equal, then it indicates an empty
        ///  queue and there must be a non-`Ok` `OutboundStatus`. We assume queues grow no greater
        ///  than 65535 items. Queue indices for normal messages begin at one; zero is reserved in
        ///  case of the need to send a high-priority signal message this block.
        ///  The bool is true if there is a signal message waiting to be sent.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.OutboundChannelDetails>> OutboundXcmpStatus(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.OutboundXcmpStatusParams();
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.OutboundChannelDetails>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> OutboundXcmpMessagesParams
        ///  The messages outbound in a given XCMP channel.
        /// </summary>
        public static string OutboundXcmpMessagesParams(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U16> key)
        {
            return RequestGenerator.GetStorage("XcmpQueue", "OutboundXcmpMessages", Substrate.NetApi.Model.Meta.Storage.Type.Map, new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                        Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat,
                        Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, key.Value);
        }
        
        /// <summary>
        /// >> OutboundXcmpMessagesDefault
        /// Default value as hex string
        /// </summary>
        public static string OutboundXcmpMessagesDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> OutboundXcmpMessages
        ///  The messages outbound in a given XCMP channel.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>> OutboundXcmpMessages(Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U16> key, CancellationToken token)
        {
            string parameters = XcmpQueueStorage.OutboundXcmpMessagesParams(key);
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> SignalMessagesParams
        ///  Any signal messages waiting to be sent.
        /// </summary>
        public static string SignalMessagesParams(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id key)
        {
            return RequestGenerator.GetStorage("XcmpQueue", "SignalMessages", Substrate.NetApi.Model.Meta.Storage.Type.Map, new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                        Substrate.NetApi.Model.Meta.Storage.Hasher.BlakeTwo128Concat}, new Substrate.NetApi.Model.Types.IType[] {
                        key});
        }
        
        /// <summary>
        /// >> SignalMessagesDefault
        /// Default value as hex string
        /// </summary>
        public static string SignalMessagesDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> SignalMessages
        ///  Any signal messages waiting to be sent.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>> SignalMessages(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id key, CancellationToken token)
        {
            string parameters = XcmpQueueStorage.SignalMessagesParams(key);
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> QueueConfigParams
        ///  The configuration which controls the dynamics of the outbound queue.
        /// </summary>
        public static string QueueConfigParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "QueueConfig", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> QueueConfigDefault
        /// Default value as hex string
        /// </summary>
        public static string QueueConfigDefault()
        {
            return "0x020000000500000001000000821A06000008000700C817A80402000400";
        }
        
        /// <summary>
        /// >> QueueConfig
        ///  The configuration which controls the dynamics of the outbound queue.
        /// </summary>
        public async Task<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.QueueConfigData> QueueConfig(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.QueueConfigParams();
            var result = await _client.GetStorageAsync<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_pallet_xcmp_queue.QueueConfigData>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> OverweightParams
        ///  The messages that exceeded max individual message weight budget.
        /// 
        ///  These message stay in this storage map until they are manually dispatched via
        ///  `service_overweight`.
        /// </summary>
        public static string OverweightParams(Substrate.NetApi.Model.Types.Primitive.U64 key)
        {
            return RequestGenerator.GetStorage("XcmpQueue", "Overweight", Substrate.NetApi.Model.Meta.Storage.Type.Map, new Substrate.NetApi.Model.Meta.Storage.Hasher[] {
                        Substrate.NetApi.Model.Meta.Storage.Hasher.Twox64Concat}, new Substrate.NetApi.Model.Types.IType[] {
                        key});
        }
        
        /// <summary>
        /// >> OverweightDefault
        /// Default value as hex string
        /// </summary>
        public static string OverweightDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> Overweight
        ///  The messages that exceeded max individual message weight budget.
        /// 
        ///  These message stay in this storage map until they are manually dispatched via
        ///  `service_overweight`.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>> Overweight(Substrate.NetApi.Model.Types.Primitive.U64 key, CancellationToken token)
        {
            string parameters = XcmpQueueStorage.OverweightParams(key);
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.NetApi.Model.Types.Primitive.U32, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8>>>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> CounterForOverweightParams
        /// Counter for the related counted storage map
        /// </summary>
        public static string CounterForOverweightParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "CounterForOverweight", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> CounterForOverweightDefault
        /// Default value as hex string
        /// </summary>
        public static string CounterForOverweightDefault()
        {
            return "0x00000000";
        }
        
        /// <summary>
        /// >> CounterForOverweight
        /// Counter for the related counted storage map
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Primitive.U32> CounterForOverweight(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.CounterForOverweightParams();
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Primitive.U32>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> OverweightCountParams
        ///  The number of overweight messages ever recorded in `Overweight`. Also doubles as the next
        ///  available free overweight index.
        /// </summary>
        public static string OverweightCountParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "OverweightCount", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> OverweightCountDefault
        /// Default value as hex string
        /// </summary>
        public static string OverweightCountDefault()
        {
            return "0x0000000000000000";
        }
        
        /// <summary>
        /// >> OverweightCount
        ///  The number of overweight messages ever recorded in `Overweight`. Also doubles as the next
        ///  available free overweight index.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Primitive.U64> OverweightCount(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.OverweightCountParams();
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Primitive.U64>(parameters, token);
            return result;
        }
        
        /// <summary>
        /// >> QueueSuspendedParams
        ///  Whether or not the XCMP queue is suspended from executing incoming XCMs or not.
        /// </summary>
        public static string QueueSuspendedParams()
        {
            return RequestGenerator.GetStorage("XcmpQueue", "QueueSuspended", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> QueueSuspendedDefault
        /// Default value as hex string
        /// </summary>
        public static string QueueSuspendedDefault()
        {
            return "0x00";
        }
        
        /// <summary>
        /// >> QueueSuspended
        ///  Whether or not the XCMP queue is suspended from executing incoming XCMs or not.
        /// </summary>
        public async Task<Substrate.NetApi.Model.Types.Primitive.Bool> QueueSuspended(CancellationToken token)
        {
            string parameters = XcmpQueueStorage.QueueSuspendedParams();
            var result = await _client.GetStorageAsync<Substrate.NetApi.Model.Types.Primitive.Bool>(parameters, token);
            return result;
        }
    }
    
    public sealed class XcmpQueueCalls
    {
        
        /// <summary>
        /// >> service_overweight
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method ServiceOverweight(Substrate.NetApi.Model.Types.Primitive.U64 index, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2.Weight weight_limit)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(index.Encode());
            byteArray.AddRange(weight_limit.Encode());
            return new Method(30, "XcmpQueue", 0, "service_overweight", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> suspend_xcm_execution
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method SuspendXcmExecution()
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            return new Method(30, "XcmpQueue", 1, "suspend_xcm_execution", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> resume_xcm_execution
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method ResumeXcmExecution()
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            return new Method(30, "XcmpQueue", 2, "resume_xcm_execution", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_suspend_threshold
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateSuspendThreshold(Substrate.NetApi.Model.Types.Primitive.U32 @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 3, "update_suspend_threshold", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_drop_threshold
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateDropThreshold(Substrate.NetApi.Model.Types.Primitive.U32 @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 4, "update_drop_threshold", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_resume_threshold
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateResumeThreshold(Substrate.NetApi.Model.Types.Primitive.U32 @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 5, "update_resume_threshold", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_threshold_weight
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateThresholdWeight(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2.Weight @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 6, "update_threshold_weight", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_weight_restrict_decay
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateWeightRestrictDecay(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2.Weight @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 7, "update_weight_restrict_decay", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> update_xcmp_max_individual_weight
        /// Contains a variant per dispatchable extrinsic that this pallet has.
        /// </summary>
        public static Method UpdateXcmpMaxIndividualWeight(Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_weights.weight_v2.Weight @new)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(@new.Encode());
            return new Method(30, "XcmpQueue", 8, "update_xcmp_max_individual_weight", byteArray.ToArray());
        }
    }
    
    public sealed class XcmpQueueConstants
    {
    }
    
    public enum XcmpQueueErrors
    {
        
        /// <summary>
        /// >> FailedToSend
        /// Failed to send XCM message.
        /// </summary>
        FailedToSend,
        
        /// <summary>
        /// >> BadXcmOrigin
        /// Bad XCM origin.
        /// </summary>
        BadXcmOrigin,
        
        /// <summary>
        /// >> BadXcm
        /// Bad XCM data.
        /// </summary>
        BadXcm,
        
        /// <summary>
        /// >> BadOverweightIndex
        /// Bad overweight index.
        /// </summary>
        BadOverweightIndex,
        
        /// <summary>
        /// >> WeightOverLimit
        /// Provided weight is possibly not enough to execute the message.
        /// </summary>
        WeightOverLimit,
    }
}
