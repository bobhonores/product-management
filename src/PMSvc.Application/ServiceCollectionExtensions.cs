using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PMSvc.Application.Behaviors;
using System.Diagnostics.CodeAnalysis;

namespace PMSvc.Application
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies(new[] { typeof(ServiceCollectionExtensions).Assembly });

            services.AddBehaviors();

            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }

        public static IServiceCollection AddBehaviors(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
