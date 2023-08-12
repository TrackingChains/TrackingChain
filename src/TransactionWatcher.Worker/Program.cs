using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrackingChain.Common.Interfaces;
using TrackingChain.Core;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionWatcherCore.Services;
using TrackingChain.TransactionWatcherCore.UseCases;
using TrackingChain.TransactionWatcherWorker.Options;
using TransactionWatcherWorker;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //config
        var databaseSection = hostContext.Configuration.GetSection("Database");
        services.Configure<DatabaseOptions>(databaseSection);
        var checkerOptions = hostContext.Configuration.GetSection("Checker");
        services.Configure<CheckerOptions>(checkerOptions);

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IBlockchainService, NethereumService>();
        services.AddTransient<IBlockchainService, SubstrateGenericClient>();
        services.AddTransient<IPendingTransactionWatcherUseCase, PendingTransactionWatcherUseCase>();
        services.AddTransient<ITransactionWatcherService, TransactionWatcherService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<PendingTransactionCheckerWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();

host.Run();
