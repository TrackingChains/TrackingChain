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


namespace Substrate.ContractRococo.NET.NetApiExt.Generated.Types.Base
{
    
    
    /// <summary>
    /// >> 146 - Composite[BTreeMapT1]
    /// </summary>
    [SubstrateNodeType(TypeDefEnum.Composite)]
    public sealed class BTreeMapT1 : BaseType
    {
        
        /// <summary>
        /// >> value
        /// </summary>
        private Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_primitives_parachain_inherent.MessageQueueChain>> _value;
        
        public Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_primitives_parachain_inherent.MessageQueueChain>> Value
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
            return "BTreeMapT1";
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
            Value = new Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.polkadot_parachain.primitives.Id, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.cumulus_primitives_parachain_inherent.MessageQueueChain>>();
            Value.Decode(byteArray, ref p);
            var bytesLength = p - start;
            TypeSize = bytesLength;
            Bytes = new byte[bytesLength];
            System.Array.Copy(byteArray, start, Bytes, 0, bytesLength);
        }
    }
}