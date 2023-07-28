using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrackingChain.AggregatorPoolCore.Services;
using TrackingChain.AggregatorPoolCore.UseCases;
using TrackingChain.AggregatorPoolWorker;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //config
        var databaseConfigSection = hostContext.Configuration.GetSection("Database");
        services.Configure<DatabaseOptions>(databaseConfigSection);

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IAggregatorService, AggregatorService>();
        services.AddTransient<IEnqueuerPoolUseCase, EnqueuerPoolUseCase>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<PoolEnqueuerWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();

host.Run();
