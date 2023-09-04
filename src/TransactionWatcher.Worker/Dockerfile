#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TransactionWatcher.Worker/TransactionWatcher.Worker.csproj", "src/TransactionWatcher.Worker/"]
COPY ["src/EVM.Generic.Client/EVM.Generic.Client.csproj", "src/EVM.Generic.Client/"]
COPY ["src/TrackingChain.Core/TrackingChain.Core.csproj", "src/TrackingChain.Core/"]
COPY ["src/TrackingChain.Common/TrackingChain.Common.csproj", "src/TrackingChain.Common/"]
COPY ["src/Substrate.Generic.Client/Substrate.Generic.Client.csproj", "src/Substrate.Generic.Client/"]
COPY ["src/Substrate.ContractRococo.NET.NetApiExt/Substrate.ContractRococo.NET.NetApiExt.csproj", "src/Substrate.ContractRococo.NET.NetApiExt/"]
COPY ["src/Substrate.NetApi/Substrate.NetApi.csproj", "src/Substrate.NetApi/"]
COPY ["src/Substrate.Shibuya.NET.NetApiExt/Substrate.Shibuya.NET.NetApiExt.csproj", "src/Substrate.Shibuya.NET.NetApiExt/"]
COPY ["src/TransactionWatcher.Core/TransactionWatcher.Core.csproj", "src/TransactionWatcher.Core/"]
RUN dotnet restore "src/TransactionWatcher.Worker/TransactionWatcher.Worker.csproj"
COPY . .
WORKDIR "/src/src/TransactionWatcher.Worker"
RUN dotnet build "TransactionWatcher.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionWatcher.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionWatcher.Worker.dll"]