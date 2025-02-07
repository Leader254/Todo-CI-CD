using System.Reflection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using TodoAPI.AppDataContext;
using TodoAPI.Interfaces;
using TodoAPI.Middleware;
using TodoAPI.Models;
using TodoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.AddSingleton<TodoDbContext>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<ITodoServices, TodoServices>();

var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Azure Key Vault Configuration
if (!builder.Environment.IsDevelopment())
{
    Console.WriteLine("Fetching database connection string from Azure Key Vault...");

    var keyVaultUri = builder.Configuration["KeyVault:KeyVaultURL"];
    var clientId = builder.Configuration["KeyVault:ClientId"];
    var clientSecret = builder.Configuration["KeyVault:ClientSecret"];
    var tenantId = builder.Configuration["KeyVault:DirectoryId"];

    if (string.IsNullOrEmpty(keyVaultUri) || string.IsNullOrEmpty(clientId) ||
        string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
    {
        throw new InvalidOperationException("Key Vault credentials are missing from configuration.");
    }

    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    var secretClient = new SecretClient(new Uri(keyVaultUri), credential);

    // Fetch the connection string from Key Vault
    var secretName = "db-connection-value";
    var connectionStringSecret = secretClient.GetSecret(secretName).Value.Value;

    if (string.IsNullOrEmpty(connectionStringSecret))
    {
        throw new InvalidOperationException("Failed to retrieve database connection string from Key Vault.");
    }

    builder.Services.AddDbContext<TodoDbContext>(options =>
    {
        options.UseSqlServer(connectionStringSecret);
    });
}
else
{
    Console.WriteLine("Using local development database connection.");
    builder.Services.AddDbContext<TodoDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
}

// Other configurations
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
