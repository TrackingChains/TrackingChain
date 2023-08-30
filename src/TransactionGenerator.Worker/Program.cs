using EVM.Generic.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionGeneratorCore.Services;
using TrackingChain.TransactionGeneratorCore.UseCases;
using TrackingChain.TransactionGeneratorWorker;
using TrackingChain.TransactionGeneratorWorker.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //config
        services.Configure<DatabaseOptions>(hostContext.Configuration.GetSection("Database"));
        services.Configure<DequeuerOptions>(hostContext.Configuration.GetSection("Dequeuer"));

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IBlockchainService, NethereumService>();
        services.AddTransient<IBlockchainService, SubstrateGenericClient>();
        services.AddTransient<ITransactionGeneratorService, TransactionGeneratorService>();
        services.AddTransient<IPoolDequeuerUseCase, PoolDequeuerUseCase>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<PoolDequeuerWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();

host.Run();
