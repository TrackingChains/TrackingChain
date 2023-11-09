using EVM.Generic.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime.InteropServices;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionWatcherCore.Services;
using TrackingChain.TransactionWatcherCore.UseCases;
using TrackingChain.TransactionWatcherWorker;
using TrackingChain.TransactionWatcherWorker.Options;
using TransactionWatcherWorker;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            services.AddWindowsService(options =>
            {
                options.ServiceName = "TrackingChain TransactionWatcher Worker";
            });

        //config
        services.Configure<DatabaseOptions>(hostContext.Configuration.GetSection("Database"));
        services.Configure<CheckerOptions>(hostContext.Configuration.GetSection("Checker"));

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IBlockchainService, NethereumService>();
        services.AddTransient<IBlockchainService, SubstrateGenericClient>();
        services.AddTransient<IPendingTransactionWatcherUseCase, PendingTransactionWatcherUseCase>();
        services.AddTransient<ITransactionWatcherService, TransactionWatcherService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<WaitingDBHostedService>();
        services.AddHostedService<PendingTransactionCheckerWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();

host.Run();
