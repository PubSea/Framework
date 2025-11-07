using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RestApi.Services;
using PubSea.Framework.Attributes;
using PubSea.Framework.DomainModel;
using PubSea.Framework.Extensions;
using PubSea.Framework.Http.HealthCheck;
using PubSea.Mediator;
using PubSea.Framework.Middlewares;
using Serilog;
using Serilog.Exceptions;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Framework Testing Application start-up completed");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddControllers(config =>
    {
        config.Filters.Add<ReformatInputValidationAttribute>();
    })
    .AddJsonOptions(opts =>
    {
        var enumConverter = new JsonStringEnumConverter();
        opts.JsonSerializerOptions.Converters.Add(enumConverter);
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSeaMediator();

    builder.Services.AddSeaEventDispatcher();

    builder.Services.AddSeaFileStore(config =>
    {
        config.BaseUrl = "http://localhost:9000";
        config.UserName = "ROOTUSER";
        config.Password = "CHANGEME123";
        config.RootName = "users";
    });

    builder.Services.AddSeaRedisHybridCache(options =>
    {
        options.ConfigureRedis(r =>
        {
            r.Configuration = "localhost:6379";
            r.InstanceName = "sample__";
        });
    });

    builder.Services.AddSeaIdentityProviderSlo("localhost:6379");

    builder.Services.AddScoped<ITestService, TestService>();

    builder.Services.AddSnowflakeService(config =>
    {
        config.GeneratorId = 255;
        config.Epoch = new DateTime(2022, 9, 18, 0, 0, 0, DateTimeKind.Utc);
        config.IdStructure = (41, 10, 12);
    });
    builder.Services.AddHashIdService(config =>
    {
        config.MinHashLength = 20;
    });
    builder.Services.AddDateTimeService();

    builder.Services.AddSeaMapper();

    builder.Services.AddHealthChecks()
        .AddCurlHealthCheck([
            new HealthCheckEndpoint(){ Name = "google", Url = "https://google.com", },
        ]);

    builder.Host.UseSerilog((context, config) =>
    {
        config.Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithAssemblyName()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Http(builder.Configuration.GetValue<string>("Serilog:LogstashUri")!, queueLimitBytes: null)
            .ReadFrom.Configuration(context.Configuration);
    });

    var app = builder.Build();
    app.UseSeaMapper();
    app.UseSerilogRequestLogging();

    app.UseSeaEventDispatcher();

    app.UseMiddleware<WebErrorHandlerMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapHealthChecks("/healthz", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    });

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception occured in Framework Testing Application");
}
finally
{
    Log.Information("Framework Testing Application shut-down completed");
    Log.CloseAndFlush();
}
