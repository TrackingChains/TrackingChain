using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Serilog;
using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrackingChain.TrackingChainCore.EntityFramework;
using TrackingChain.TrackingChainCore.EntityFramework.Context;
using TrackingChain.TrackingChainCore.Options;
using TrackingChain.TransactionTriageCore.Services;
using TrackingChain.TransactionTriageCore.UseCases;
using TrackingChain.TransactionWaitingCore.Services;
using TrackingChain.TriageAPI.API;
using TrackingChain.TriageAPI.HostedService;
using TrackingChain.TriageAPI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//database
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHostedService<MigratorDBHostedService>();

//services
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IRegistryService, RegistryService>();
builder.Services.AddTransient<IAnalyticUseCase, AnalyticUseCase>();
builder.Services.AddTransient<ITrackingEntryUseCase, TrackingEntryUseCase>();
builder.Services.AddTransient<ITransactionTriageService, TransactionTriageService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//cors
var corsOption = builder.Configuration.GetSection("CORS").Get<CORSOption>();
if (corsOption?.Enable ?? false)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.SetIsOriginAllowed(i => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Token-Expired");
            });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

if (corsOption?.Enable ?? false) app.UseCors("AllowAll");

app.MapControllers();

app.Run();
