using Azure.Identity;
using LearningAzureScalableAPI.Contracts;
using LearningAzureScalableAPI.Data;
using LearningAzureScalableAPI.Repository;
using LearningAzureScalableAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContextFile>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "https://localhost:7091";
});

builder.Services.AddScoped<IOrderRepository, OrdersRepository>();
builder.Services.AddScoped<BlobStorageService>();

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpClient();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddApplicationInsights();
builder.Logging.SetMinimumLevel(LogLevel.Information);


var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/testlog", (ILogger<Program> logger) =>
{
    logger.LogInformation("This is a test log from Azure");
    logger.LogWarning("This is a warning log");
    logger.LogError("This is an error log");

    return "Logs written";
});


app.Run();
