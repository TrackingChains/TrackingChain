using EVM.Generic.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionMonitorCore.Options;
using TrackingChain.TransactionMonitorCore.Services;
using TrackingChain.TransactionMonitorWorker;
using TrackingChain.TransactionMonitorWorker.Options;
using TrackingChain.TransactionRecoveryWorker.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        //config
        services.Configure<DatabaseOptions>(hostContext.Configuration.GetSection("Database"));
        services.Configure<MonitorOptions>(hostContext.Configuration.GetSection("Monitor"));
        services.Configure<MailSettingsOption>(hostContext.Configuration.GetSection("MailSettings"));
        services.Configure<FileReportSettingsOption>(hostContext.Configuration.GetSection("FileReportSettings"));

        //database
        services.AddDbContext<ApplicationDbContext>();

        //services
        services.AddTransient<IBlockchainService, NethereumService>();
        services.AddTransient<IBlockchainService, SubstrateGenericClient>();
        ConfigureReportService(hostContext, services);

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

static void ConfigureReportService(HostBuilderContext hostContext, IServiceCollection services)
{
    services.AddTransient<IReportGeneratorService, ReportGeneratorService>();

    string? reportOutputValue = hostContext.Configuration.GetSection("ReportSettings:ReportOutput").Get<string>();
    if (Enum.TryParse<ReportOutputType>(reportOutputValue, out var reportOutput))
        switch (reportOutput)
        {
            case ReportOutputType.File:
                services.AddScoped<IAlertService, FileAlertService>();
                break;
            case ReportOutputType.Mail:
                services.AddScoped<IAlertService, MailAlertService>();
                break;
            case ReportOutputType.All:
                services.AddScoped<IAlertService, FileAlertService>();
                services.AddScoped<IAlertService, MailAlertService>();
                break;
            default:
                throw new InvalidOperationException("IAlertService configuration not found or invalid");
        }
    else
        throw new InvalidOperationException("IAlertService configuration not found or invalid");
}