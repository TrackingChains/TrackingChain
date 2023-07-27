For more info visit: 
    https://github.com/SubstrateGaming/Substrate.Chains.NET


Example fo command to add new template for specific substrate chain:

    dotnet new install Substrate.DotNet.Template
    dotnet new substrate --sdk_version 0.4.4 --rest_service Substrate.ContractRococo.NET.RestService --net_api Substrate.ContractRococo.NET.NetApiExt --rest_client Substrate.ContractRococo.NET.RestClient --metadata_websocket wss://rococo-contracts-rpc.polkadot.io --force --allow-scripts yes


