using Claims.Application.Commands;
using Claims.Application.Queries;
using Claims.Application.Services;
using Claims.Infrastructure;
using Claims.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Start Testcontainers for SQL Server and MongoDB
MsSqlContainer sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        : new()

    ).Build();

MongoDbContainer mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await sqlContainer.StartAsync();
await mongoContainer.StartAsync();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(sqlContainer.GetConnectionString()));

builder.Services.AddDbContext<ClaimsContext>(options =>
{
    MongoClient client = new MongoClient(mongoContainer.GetConnectionString());
    IMongoDatabase database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

builder.Services.AddScoped<GetClaimsQuery>();
builder.Services.AddScoped<CreateClaimCommandHandler>();
builder.Services.AddScoped<GetClaimByIdQuery>();
builder.Services.AddScoped<DeleteClaimCommandHandler>();

builder.Services.AddScoped<CreateCoverCommandHandler>();
builder.Services.AddScoped<DeleteCoverCommandHandler>();

builder.Services.AddScoped<GetCoversQuery>();
builder.Services.AddScoped<GetCoverByIdQuery>();

builder.Services.AddScoped<ComputePremiumService>();
builder.Services.AddScoped<ComputePremiumQuery>();


builder.Services.AddSingleton<IAuditQueue, InMemoryAuditQueue>();
builder.Services.AddHostedService<AuditBackgroundService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    AuditContext context1 = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context1.Database.Migrate();
}

app.Run();

public partial class Program { }
