using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration.Json;
using Mipe.Service;
using Mipe.Service.FileLogger;

var cliParser = new Parser(c => { c.IgnoreUnknownArguments = true; });
var cliResult = cliParser.ParseArguments<CommandLineArgs>(args);
if (cliResult.Value == null)
{
  Console.WriteLine(HelpText.AutoBuild(cliResult));
  Environment.Exit(1);
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
  Args = args,
  ContentRootPath = AppContext.BaseDirectory
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

foreach (var jsonConfigurationSource in builder.Configuration.Sources.OfType<JsonConfigurationSource>())
{
  Console.WriteLine(jsonConfigurationSource.Path);
}

var cachedFileLogger = new CachedFileLoggerProvider("mipe-log.txt");

builder.Services.AddSingleton<MipeLoader>();
builder.Services.AddSingleton<ILoggerProvider, CachedFileLoggerProvider>(_ => cachedFileLogger);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<MipeLoader>>();
var loader = app.Services.GetRequiredService<MipeLoader>();
try
{
  await loader.LoadConfiguration(cliResult.Value.File);
}
catch (Exception ex)
{
  logger.LogError(ex, "Loading configuration failed");
  Environment.Exit(2);
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();