using ServerAPI.Extensions;
using ServerAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

app.UseCors("Cors");

app.MapHub<ChatHub>("/chat");

app.Run();
