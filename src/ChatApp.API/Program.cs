using ChatApp.API.Extensions;
using ChatApp.API.Middlewares;
using ChatApp.Application;
using ChatApp.Application.Hubs;
using ChatApp.Infrastructure;
using NLog;

try 
{
    var builder = WebApplication.CreateBuilder(args);

    // Custom Logging
    builder.Logging.AddLogger(builder.Configuration);
    // Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddInfrastructure(builder.Configuration)
        .AddApplication();
    builder.Services.AddAppServices();

    var app = await builder.Build().MigrateDatabaseAsync();

    app.UseMiddleware<ExceptionMiddleware>();
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
