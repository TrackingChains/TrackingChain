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


namespace Substrate.Shibuya.NET.NetApiExt.Generated.Model.fp_self_contained.unchecked_extrinsic
{
    
    
    /// <summary>
    /// >> 533 - Composite[fp_self_contained.unchecked_extrinsic.UncheckedExtrinsic]
    /// </summary>
    [SubstrateNodeType(TypeDefEnum.Composite)]
    public sealed class UncheckedExtrinsic : BaseType
    {
        
        /// <summary>
        /// >> value
        /// </summary>
        private Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.generic.unchecked_extrinsic.UncheckedExtrinsic _value;
        
        public Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.generic.unchecked_extrinsic.UncheckedExtrinsic Value
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
            return "UncheckedExtrinsic";
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
            Value = new Substrate.Shibuya.NET.NetApiExt.Generated.Model.sp_runtime.generic.unchecked_extrinsic.UncheckedExtrinsic();
            Value.Decode(byteArray, ref p);
            var bytesLength = p - start;
            TypeSize = bytesLength;
            Bytes = new byte[bytesLength];
            System.Array.Copy(byteArray, start, Bytes, 0, bytesLength);
        }
    }
}
