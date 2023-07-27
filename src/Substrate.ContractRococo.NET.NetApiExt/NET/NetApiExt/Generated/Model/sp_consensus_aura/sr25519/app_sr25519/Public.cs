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


namespace Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_consensus_aura.sr25519.app_sr25519
{
    
    
    /// <summary>
    /// >> 193 - Composite[sp_consensus_aura.sr25519.app_sr25519.Public]
    /// </summary>
    [SubstrateNodeType(TypeDefEnum.Composite)]
    public sealed class Public : BaseType
    {
        
        /// <summary>
        /// >> value
        /// </summary>
        private Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.sr25519.Public _value;
        
        public Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.sr25519.Public Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
        
        public override string TypeName()
        {
            return "Public";
        }
        
        public override byte[] Encode()
        {
            var result = new List<byte>();
            result.AddRange(Value.Encode());
            return result.ToArray();
        }
        
        public override void Decode(byte[] byteArray, ref int p)
        {
            var start = p;
            Value = new Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.sr25519.Public();
            Value.Decode(byteArray, ref p);
            var bytesLength = p - start;
            TypeSize = bytesLength;
            Bytes = new byte[bytesLength];
            System.Array.Copy(byteArray, start, Bytes, 0, bytesLength);
        }
    }
}
