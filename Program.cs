using System.Reflection;
using TodoAPI.AppDataContext;
using TodoAPI.Interfaces;
using TodoAPI.Middleware;
using TodoAPI.Models;
using TodoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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


// builder.Services.AddExceptionHandler(options =>
// {
//     options.ExceptionHandlingPath = "/error";
// });

builder.Services.AddProblemDetails();
builder.Services.AddLogging();

var app = builder.Build();
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider;
}
app.UseSwagger();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
