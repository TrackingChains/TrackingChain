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
    
    
    public sealed class EthereumCheckedStorage
    {
        
        // Substrate client for the storage calls.
        private SubstrateClientExt _client;
        
        public EthereumCheckedStorage(SubstrateClientExt client)
        {
            this._client = client;
            _client.StorageKeyDict.Add(new System.Tuple<string, string>("EthereumChecked", "Nonce"), new System.Tuple<Substrate.NetApi.Model.Meta.Storage.Hasher[], System.Type, System.Type>(null, null, typeof(Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256)));
        }
        
        /// <summary>
        /// >> NonceParams
        ///  Global nonce for all transactions to avoid hash collision, which is
        ///  caused by the same dummy signatures for all transactions.
        /// </summary>
        public static string NonceParams()
        {
            return RequestGenerator.GetStorage("EthereumChecked", "Nonce", Substrate.NetApi.Model.Meta.Storage.Type.Plain);
        }
        
        /// <summary>
        /// >> NonceDefault
        /// Default value as hex string
        /// </summary>
        public static string NonceDefault()
        {
            return "0x0000000000000000000000000000000000000000000000000000000000000000";
        }
        
        /// <summary>
        /// >> Nonce
        ///  Global nonce for all transactions to avoid hash collision, which is
        ///  caused by the same dummy signatures for all transactions.
        /// </summary>
        public async Task<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256> Nonce(CancellationToken token)
        {
            string parameters = EthereumCheckedStorage.NonceParams();
            var result = await _client.GetStorageAsync<Substrate.Shibuya.NET.NetApiExt.Generated.Model.primitive_types.U256>(parameters, token);
            return result;
        }
    }
    
    public sealed class EthereumCheckedCalls
    {
        
        /// <summary>
        /// >> transact
        /// Contains one variant per dispatchable that can be called by an extrinsic.
        /// </summary>
        public static Method Transact(Substrate.Shibuya.NET.NetApiExt.Generated.Model.astar_primitives.ethereum_checked.CheckedEthereumTx tx)
        {
            System.Collections.Generic.List<byte> byteArray = new List<byte>();
            byteArray.AddRange(tx.Encode());
            return new Method(64, "EthereumChecked", 0, "transact", byteArray.ToArray());
        }
    }
    
    public sealed class EthereumCheckedConstants
    {
    }
}
