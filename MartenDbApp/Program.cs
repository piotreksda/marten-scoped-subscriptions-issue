using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using MartenDbApp;
using MartenDbApp.Events;
using MartenDbApp.Projections;
using MartenDbApp.Subscriptions;
using Npgsql;
using Oakton;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SomeScopedServiceWithQuerySession>();

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
    options.DisableNpgsqlLogging = false;
    options.Events.AddEventType<EventA>();
    options.Projections.Add<EventAProjection>(ProjectionLifecycle.Inline);

    options.Events.DatabaseSchemaName = "events_marten";
    options.DatabaseSchemaName = "documents_marten";

    options.Policies.ForAllDocuments(
        x =>
        {
            x.Metadata.CausationId.Enabled = true;
            x.Metadata.CorrelationId.Enabled = true;
            x.Metadata.Headers.Enabled = true;
        }
    );
})
    .AddAsyncDaemon(DaemonMode.HotCold)
    .ApplyAllDatabaseChangesOnStartup()
    .AddSubscriptionWithServices<WorkingTestSubscription>(ServiceLifetime.Scoped);
    // .AddSubscriptionWithServices<NotWorkingTestSubscription>(ServiceLifetime.Scoped);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-test-event", async (IDocumentSession session) =>
    {
        var @event = new EventA("Sample Test");
        session.Events.StartStream<EventA>(@event);
        await session.SaveChangesAsync();
        return "Event created and changes saved";
    })
    .WithName("create test event")
    .WithOpenApi();

await CreateDatabaseIfNotExistAsync();

await app.RunOaktonCommands(args);



async Task CreateDatabaseIfNotExistAsync()
{
    var connectionStringBuilder = new NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection")!);
    var databaseName = connectionStringBuilder.Database;
    connectionStringBuilder.Database = "postgres";
    await using var connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
    await connection.OpenAsync();

    await using var queryCmd = new NpgsqlCommand(
        $"SELECT datname FROM pg_database WHERE datname = '{databaseName}'",
        connection
    );

    var queryResult = await queryCmd.ExecuteScalarAsync();

    if (queryResult is null)
    {
        await using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
        await createCmd.ExecuteNonQueryAsync();
    }
}
