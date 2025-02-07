using System.Reflection;
using TodoAPI.AppDataContext;
using TodoAPI.Interfaces;
using TodoAPI.Middleware;
using TodoAPI.Models;
using TodoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Check the environment
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Running in {environment} mode.");

// Configure DbSettings
var dbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>();

if (environment == "Production")
{
    Console.WriteLine("Fetching database connection string from environment variable...");
    dbSettings.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? dbSettings.ConnectionString;
}

// Register the final DbSettings
builder.Services.Configure<DbSettings>(options =>
{
    options.ConnectionString = dbSettings.ConnectionString;
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<TodoDbContext>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<ITodoServices, TodoServices>();

var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddProblemDetails();
builder.Services.AddLogging();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();
