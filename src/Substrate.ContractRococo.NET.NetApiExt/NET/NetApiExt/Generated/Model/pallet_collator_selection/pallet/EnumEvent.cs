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


namespace Substrate.ContractRococo.NET.NetApiExt.Generated.Model.pallet_collator_selection.pallet
{
    
    
    public enum Event
    {
        
        NewInvulnerables = 0,
        
        InvulnerableAdded = 1,
        
        InvulnerableRemoved = 2,
        
        NewDesiredCandidates = 3,
        
        NewCandidacyBond = 4,
        
        CandidateAdded = 5,
        
        CandidateRemoved = 6,
    }
    
    /// <summary>
    /// >> 35 - Variant[pallet_collator_selection.pallet.Event]
    /// The `Event` enum of this pallet
    /// </summary>
    public sealed class EnumEvent : BaseEnumExt<Event, Substrate.NetApi.Model.Types.Base.BaseVec<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Substrate.NetApi.Model.Types.Primitive.U32, Substrate.NetApi.Model.Types.Primitive.U128, BaseTuple<Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32, Substrate.NetApi.Model.Types.Primitive.U128>, Substrate.ContractRococo.NET.NetApiExt.Generated.Model.sp_core.crypto.AccountId32>
    {
    }
}