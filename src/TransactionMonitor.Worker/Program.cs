using EVM.Generic.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorWorker;
using TrackingChain.TransactionRecoveryWorker.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //config
        var databaseSection = hostContext.Configuration.GetSection("Database");
        services.Configure<DatabaseOptions>(databaseSection);
        var recoveryOptions = hostContext.Configuration.GetSection("Monitor");
        services.Configure<MonitorOptions>(recoveryOptions);

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IBlockchainService, NethereumService>();
        services.AddTransient<IBlockchainService, SubstrateGenericClient>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<TransactionDeleterWorker>();
        services.AddHostedService<TransactionFailedWorker>();
        services.AddHostedService<TransactionLockedWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();

host.Run();

