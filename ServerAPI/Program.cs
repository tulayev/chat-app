using NLog;
using ServerAPI.Extensions;
using ServerAPI.Hubs;

try 
{
    var builder = WebApplication.CreateBuilder(args);

    // Custom Logging
    builder.Logging.AddLogger(builder.Configuration);
    // Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAppServices(builder.Configuration);

    var app = builder.Build();

    app.UseCors("Cors");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<ChatHub>("hubs/chat");

    app.Run();
}
catch (Exception ex)
{
    LogManager.GetCurrentClassLogger().Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    LogManager.Shutdown();
}
