namespace ChatApp.API.Extensions
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            return services;
        }
    }
}
