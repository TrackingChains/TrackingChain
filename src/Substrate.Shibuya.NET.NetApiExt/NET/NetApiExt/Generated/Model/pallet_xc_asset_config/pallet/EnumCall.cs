//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Substrate.NetApi.Model.Types.Base;
using System.Collections.Generic;


namespace Substrate.Shibuya.NET.NetApiExt.Generated.Model.pallet_xc_asset_config.pallet
{
    
    
    public enum Call
    {
        
        register_asset_location = 0,
        
        set_asset_units_per_second = 1,
        
        change_existing_asset_location = 2,
        
        remove_payment_asset = 3,
        
        remove_asset = 4,
    }
    
    /// <summary>
    /// >> 276 - Variant[pallet_xc_asset_config.pallet.Call]
    /// Contains one variant per dispatchable that can be called by an extrinsic.
    /// </summary>
    public sealed class EnumCall : BaseEnumExt<Call, BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.xcm.EnumVersionedMultiLocation, Substrate.NetApi.Model.Types.Base.BaseCom<Substrate.NetApi.Model.Types.Primitive.U128>>, BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.xcm.EnumVersionedMultiLocation, Substrate.NetApi.Model.Types.Base.BaseCom<Substrate.NetApi.Model.Types.Primitive.U128>>, BaseTuple<Substrate.Shibuya.NET.NetApiExt.Generated.Model.xcm.EnumVersionedMultiLocation, Substrate.NetApi.Model.Types.Base.BaseCom<Substrate.NetApi.Model.Types.Primitive.U128>>, Substrate.Shibuya.NET.NetApiExt.Generated.Model.xcm.EnumVersionedMultiLocation, Substrate.NetApi.Model.Types.Base.BaseCom<Substrate.NetApi.Model.Types.Primitive.U128>>
    {
    }
}
