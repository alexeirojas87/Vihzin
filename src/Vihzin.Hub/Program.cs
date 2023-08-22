using Vihzin.Hubs;

var builder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
builder.Services.AddSignalR();
builder.Services.AddCors();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseCors(builder =>
    builder.AllowAnyHeader()
.AllowAnyMethod()
.SetIsOriginAllowed((host) => true)
.AllowCredentials());
app.MapHub<VideoHub>("/vihzinHub");
app.MapHealthChecks("/healthz");
app.Run();