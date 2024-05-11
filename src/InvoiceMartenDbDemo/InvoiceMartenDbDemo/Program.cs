using InvoiceMartenDbDemo.Projections;
using Marten.Events.Daemon.Resiliency;
using Marten;
using Marten.Events.Projections;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    options.Projections.Snapshot<ClientDemographic>(SnapshotLifecycle.Inline);
    options.Schema.For<ClientDemographic>().Identity(x => x.ClientId);
    options.Projections.Add<UnBilledInvoiceProjection>(ProjectionLifecycle.Async);
    options.Projections.Add<BilledInvoiceProjection>(ProjectionLifecycle.Async);
    options.Projections.Add<ClientLedgerProjection>(ProjectionLifecycle.Async);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
}).AddAsyncDaemon(DaemonMode.Solo);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
