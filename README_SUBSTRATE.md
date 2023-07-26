For more info visit: 
    https://github.com/SubstrateGaming/Substrate.Chains.NET


Example fo command to add new template for specific substrate chain:

    dotnet new install Substrate.DotNet.Template
    dotnet new substrate --sdk_version 0.4.4 --rest_service Substrate.Shibuya.NET.RestService --net_api Substrate.Shibuya.NET.NetApiExt --rest_client Substrate.Shibuya.NET.RestClient --metadata_websocket wss://shibuya.blastapi.io/8508719b-69eb-4c01-92e4-eb902da0fb97 --force --allow-scripts yes


