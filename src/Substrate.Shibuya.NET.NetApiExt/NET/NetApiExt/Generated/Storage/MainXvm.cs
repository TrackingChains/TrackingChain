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


namespace Substrate.Shibuya.NET.NetApiExt.Generated.Storage
{
    
    
    public sealed class XvmStorage
    {
        
        // Substrate client for the storage calls.
        private SubstrateClientExt _client;
        
        public XvmStorage(SubstrateClientExt client)
        {
            this._client = client;
        }
    }
    
    public sealed class XvmCalls
    {
        
        /// <summary>
        /// >> xvm_call
        /// Contains one variant per dispatchable that can be called by an extrinsic.
        /// </summary>
        public static Method XvmCall(Substrate.Shibuya.NET.NetApiExt.Generated.Model.pallet_xvm.XvmContext context, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8> to, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8> input)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(context.Encode());
            byteArray.AddRange(to.Encode());
            byteArray.AddRange(input.Encode());
            return new Method(90, "Xvm", 0, "xvm_call", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> xvm_send
        /// Contains one variant per dispatchable that can be called by an extrinsic.
        /// </summary>
        public static Method XvmSend(Substrate.Shibuya.NET.NetApiExt.Generated.Model.pallet_xvm.XvmContext context, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8> to, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Primitive.U8> message)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(context.Encode());
            byteArray.AddRange(to.Encode());
            byteArray.AddRange(message.Encode());
            return new Method(90, "Xvm", 1, "xvm_send", byteArray.ToArray());
        }
        
        /// <summary>
        /// >> xvm_query
        /// Contains one variant per dispatchable that can be called by an extrinsic.
        /// </summary>
        public static Method XvmQuery(Substrate.Shibuya.NET.NetApiExt.Generated.Model.pallet_xvm.XvmContext context)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(context.Encode());
            return new Method(90, "Xvm", 2, "xvm_query", byteArray.ToArray());
        }
    }
    
    public sealed class XvmConstants
    {
    }
    
    public enum XvmErrors
    {
    }
}