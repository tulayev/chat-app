namespace ServerAPI.Extensions
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:4200",
                            "http://localhost:4201"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            return services;
        }
    }
}
