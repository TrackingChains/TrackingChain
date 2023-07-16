#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AggregatorPoolWorker/AggregatorPoolWorker.csproj", "src/AggregatorPoolWorker/"]
COPY ["src/AggregatorPoolCore/AggregatorPoolCore.csproj", "src/AggregatorPoolCore/"]
COPY ["src/TrackingChainCommon/TrackingChainCore.csproj", "src/TrackingChainCommon/"]
RUN dotnet restore "src/AggregatorPoolWorker/AggregatorPoolWorker.csproj"
COPY . .
WORKDIR "/src/src/AggregatorPoolWorker"
RUN dotnet build "AggregatorPoolWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AggregatorPoolWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AggregatorPoolWorker.dll"]