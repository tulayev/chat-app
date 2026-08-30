using Asp.Versioning.ApiExplorer;
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

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseCors("Cors");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<ChatHub>("hubs/chat").RequireAuthorization();

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
