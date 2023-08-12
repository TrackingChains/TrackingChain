using EVM.Generic.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json.Serialization;
using TrackingChain.Common.Interfaces;
using TrackingChain.Substrate.Generic.Client;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.Services;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TransactionWaitingCore.Services;
using TrackingChain.TriageWebApplication.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//serializer
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

//config
var databaseConfigSection = builder.Configuration.GetSection("Database");
builder.Services.Configure<DatabaseOptions>(databaseConfigSection);

//loggin
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//database
builder.Services.AddDbContext<ApplicationDbContext>();
//builder.Services.AddHostedService<MigratorDBHostedService>();

builder.Services.AddTransient<IAnalyticUseCase, AnalyticUseCase>();
builder.Services.AddTransient<IBlockchainService, NethereumService>();
builder.Services.AddTransient<IBlockchainService, SubstrateGenericClient>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IRegistryService, RegistryService>();
builder.Services.AddTransient<ITrackingEntryUseCase, TrackingEntryUseCase>();
builder.Services.AddTransient<ITransactionTriageService, TransactionTriageService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//cors
var corsConfig = configuration.GetSection("CORS").Get<CORSOption>();
if (corsConfig?.Enable ?? false)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.SetIsOriginAllowed(i => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

if (corsConfig?.Enable ?? false) app.UseCors("AllowAll");

app.UseRouting();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations
app.MapControllers();
app.MapRazorPages();

app.Run();
