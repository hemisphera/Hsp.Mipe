using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Primitives;
using Mipe.Core.FileLogger;
using Mipe.Service;
using Mipe.Service.FileLogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

foreach (var jsonConfigurationSource in builder.Configuration.Sources.OfType<JsonConfigurationSource>())
{
  Console.WriteLine(jsonConfigurationSource.Path);
}

builder.Services.Configure<MipeServiceSettings>(builder.Configuration.GetSection("settings"));

var cachedFileLogger = new CachedFileLoggerProvider("mipe-log.txt");

builder.Services.AddSingleton<MipeLoader>();
builder.Services.AddSingleton<ILoggerProvider, CachedFileLoggerProvider>(c => cachedFileLogger);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseHttpsRedirection();

var loader = app.Services.GetRequiredService<MipeLoader>();
loader.Listen();
await loader.LoadConfiguration();

//app.Lifetime.ApplicationStopped.Register(() => { cachedFileLogger.Dispose(); });

app.Run();