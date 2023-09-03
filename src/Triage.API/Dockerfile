#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Triage.API/Triage.API.csproj", "src/Triage.API/"]
COPY ["src/TransactionTriage.Core/TransactionTriage.Core.csproj", "src/TransactionTriage.Core/"]
COPY ["src/TrackingChain.Core/TrackingChain.Core.csproj", "src/TrackingChain.Core/"]
COPY ["src/TrackingChain.Common/TrackingChain.Common.csproj", "src/TrackingChain.Common/"]
RUN dotnet restore "src/Triage.API/Triage.API.csproj"
COPY . .
WORKDIR "/src/src/Triage.API"
RUN dotnet build "Triage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Triage.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triage.API.dll"]