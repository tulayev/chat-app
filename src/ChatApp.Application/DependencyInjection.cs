using ChatApp.Application.Common.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ChatApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) 
        {
            // SignalR
            services.AddSignalR();
            // Mapster
            var mapsterConfig = new TypeAdapterConfig();
            mapsterConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper, ServiceMapper>();
            // CQRS
            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Validation Pipeline
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
