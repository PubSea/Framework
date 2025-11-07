using MessagingEfKafka;
using Microsoft.EntityFrameworkCore;
using PubSea.Framework.Data;
using PubSea.Messaging.EntityFramework;
using PubSea.Messaging.EntityFramework.Kafka.Configs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContextPool<AppDbContext>((provider, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), options =>
    {
        options.MigrationsHistoryTable("ef_migration_history");
        options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), []);
    })
    .UseSeaHeavyQueryInterceptor(provider)
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddSeaMessaging<AppDbContext>(config =>
{
    config.PublishOutboxInstantly = true;
    config.OutboxPollingInterval = TimeSpan.FromSeconds(30);

    config.UseKafkaBroker(config =>
    {
        config.ConnectionString = builder.Configuration.GetConnectionString("Kafka")!;
        config.ClientId = "TestApp";
        config.TopicName = "messaging_test_outbox";
        config.ConsumingTopicNames = ["messaging_test_outbox",];
        config.ConsumerGroupId = "TestAppConsumerGroup";
        config.ConcurrentConsumers = 2;
    });

    //config.UseRabbitMqBroker(config =>
    //{
    //    config.ClientId = "messaging_test";
    //    config.ConnectionString = builder.Configuration.GetConnectionString("RabbitMq")!;
    //});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
