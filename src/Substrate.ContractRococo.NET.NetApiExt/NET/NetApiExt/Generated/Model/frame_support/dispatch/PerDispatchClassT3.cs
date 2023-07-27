//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Substrate.NetApi.Attributes;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Metadata.V14;
using System.Collections.Generic;


namespace Substrate.ContractRococo.NET.NetApiExt.Generated.Model.frame_support.dispatch
{
    
    
    /// <summary>
    /// >> 124 - Composite[frame_support.dispatch.PerDispatchClassT3]
    /// </summary>
    [SubstrateNodeType(TypeDefEnum.Composite)]
    public sealed class PerDispatchClassT3 : BaseType
    {
        
        /// <summary>
        /// >> normal
        /// </summary>
        private Substrate.NetApi.Model.Types.Primitive.U32 _normal;
        
        /// <summary>
        /// >> operational
        /// </summary>
        private Substrate.NetApi.Model.Types.Primitive.U32 _operational;
        
        /// <summary>
        /// >> mandatory
        /// </summary>
        private Substrate.NetApi.Model.Types.Primitive.U32 _mandatory;
        
        public Substrate.NetApi.Model.Types.Primitive.U32 Normal
        {
            get
            {
                return this._normal;
            }
            set
            {
                this._normal = value;
            }
        }
        
        public Substrate.NetApi.Model.Types.Primitive.U32 Operational
        {
            get
            {
                return this._operational;
            }
            set
            {
                this._operational = value;
            }
        }
        
        public Substrate.NetApi.Model.Types.Primitive.U32 Mandatory
        {
            get
            {
                return this._mandatory;
            }
            set
            {
                this._mandatory = value;
            }
        }
        
        public override string TypeName()
        {
            return "PerDispatchClassT3";
        }
        
        public override byte[] Encode()
        {
            var result = new List<byte>();
            result.AddRange(Normal.Encode());
            result.AddRange(Operational.Encode());
            result.AddRange(Mandatory.Encode());
            return result.ToArray();
        }
        
        public override void Decode(byte[] byteArray, ref int p)
        {
            var start = p;
            Normal = new Substrate.NetApi.Model.Types.Primitive.U32();
            Normal.Decode(byteArray, ref p);
            Operational = new Substrate.NetApi.Model.Types.Primitive.U32();
            Operational.Decode(byteArray, ref p);
            Mandatory = new Substrate.NetApi.Model.Types.Primitive.U32();
            Mandatory.Decode(byteArray, ref p);
            var bytesLength = p - start;
            TypeSize = bytesLength;
            Bytes = new byte[bytesLength];
            System.Array.Copy(byteArray, start, Bytes, 0, bytesLength);
        }
    }
}
