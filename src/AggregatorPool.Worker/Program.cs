using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using TrackingChain.AggregatorPoolCore.Services;
using TrackingChain.AggregatorPoolCore.UseCases;
using TrackingChain.AggregatorPoolWorker;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            services.AddWindowsService(options =>
            {
                options.ServiceName = "TrackingChain AggregatorPool Worker";
            });

        //config
        services.Configure<DatabaseOptions>(hostContext.Configuration.GetSection("Database"));

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IAggregatorService, AggregatorService>();
        services.AddTransient<IEnqueuerPoolUseCase, EnqueuerPoolUseCase>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddHostedService<WaitingDBHostedService>();
        services.AddHostedService<PoolEnqueuerWorker>();
    })
    .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext())
    .Build();



host.Run();
