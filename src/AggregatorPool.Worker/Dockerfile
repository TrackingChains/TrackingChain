#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AggregatorPool.Worker/AggregatorPool.Worker.csproj", "src/AggregatorPool.Worker/"]
COPY ["src/AggregatorPool.Core/AggregatorPool.Core.csproj", "src/AggregatorPool.Core/"]
COPY ["src/TrackingChain.Core/TrackingChain.Core.csproj", "src/TrackingChain.Core/"]
COPY ["src/TrackingChain.Common/TrackingChain.Common.csproj", "src/TrackingChain.Common/"]
RUN dotnet restore "src/AggregatorPool.Worker/AggregatorPool.Worker.csproj"
COPY . .
WORKDIR "/src/src/AggregatorPool.Worker"
RUN dotnet build "AggregatorPool.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AggregatorPool.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregatorPool.Worker.dll"]