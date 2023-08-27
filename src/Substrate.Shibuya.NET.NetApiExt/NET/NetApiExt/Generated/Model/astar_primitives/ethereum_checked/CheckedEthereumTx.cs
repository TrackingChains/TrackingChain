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


namespace Substrate.Shibuya.NET.NetApiExt.Generated.Model.astar_primitives.ethereum_checked
{
    
    
    /// <summary>
    /// >> 297 - Composite[astar_primitives.ethereum_checked.CheckedEthereumTx]
    /// </summary>
    [SubstrateNodeType(TypeDefEnum.Composite)]
    public sealed class CheckedEthereumTx : BaseType
    {
        
        /// <summary>
        /// >> gas_limit
        /// </summary>
        private Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256 _gasLimit;
        
        /// <summary>
        /// >> target
        /// </summary>
        private Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160 _target;
        
        /// <summary>
        /// >> value
        /// </summary>
        private Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256 _value;
        
        /// <summary>
        /// >> input
        /// </summary>
        private Substrate.Shibuya.NET.NetApiExt.Generated.Model.bounded_collections.bounded_vec.BoundedVecT5 _input;
        
        /// <summary>
        /// >> maybe_access_list
        /// </summary>
        private Substrate.NetApi.Model.Types.Base.BaseOpt<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H256>>>> _maybeAccessList;
        
        public Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256 GasLimit
        {
            get
            {
                return this._gasLimit;
            }
            set
            {
                this._gasLimit = value;
            }
        }
        
        public Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160 Target
        {
            get
            {
                return this._target;
            }
            set
            {
                this._target = value;
            }
        }
        
        public Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256 Value
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
        
        public Substrate.Shibuya.NET.NetApiExt.Generated.Model.bounded_collections.bounded_vec.BoundedVecT5 Input
        {
            get
            {
                return this._input;
            }
            set
            {
                this._input = value;
            }
        }
        
        public Substrate.NetApi.Model.Types.Base.BaseOpt<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H256>>>> MaybeAccessList
        {
            get
            {
                return this._maybeAccessList;
            }
            set
            {
                this._maybeAccessList = value;
            }
        }
        
        public override string TypeName()
        {
            return "CheckedEthereumTx";
        }
        
        public override byte[] Encode()
        {
            var result = new List<byte>();
            result.AddRange(GasLimit.Encode());
            result.AddRange(Target.Encode());
            result.AddRange(Value.Encode());
            result.AddRange(Input.Encode());
            result.AddRange(MaybeAccessList.Encode());
            return result.ToArray();
        }
        
        public override void Decode(byte[] byteArray, ref int p)
        {
            var start = p;
            GasLimit = new Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256();
            GasLimit.Decode(byteArray, ref p);
            Target = new Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160();
            Target.Decode(byteArray, ref p);
            Value = new Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256();
            Value.Decode(byteArray, ref p);
            Input = new Substrate.Shibuya.NET.NetApiExt.Generated.Model.bounded_collections.bounded_vec.BoundedVecT5();
            Input.Decode(byteArray, ref p);
            MaybeAccessList = new Substrate.NetApi.Model.Types.Base.BaseOpt<Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.NetApi.Model.Types.Base.BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H160, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.H256>>>>();
            MaybeAccessList.Decode(byteArray, ref p);
            var bytesLength = p - start;
            TypeSize = bytesLength;
            Bytes = new byte[bytesLength];
            System.Array.Copy(byteArray, start, Bytes, 0, bytesLength);
        }
    }
}