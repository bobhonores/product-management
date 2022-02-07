using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PMSvc.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseConfigurations(configuration);

            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddTransient<IProductQueryRepository, ProductQueryRepository>();

            services.AddTransient<IReviewQueryRepository, ReviewQueryRepository>();

            services.AddProblemDetails(ConfigureProblemDetails);

            services.AddExternalServices(configuration);

            services.AddLazyCache();

            return services;
        }

        private static IServiceCollection AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductDbContext>(options => options.UseSqlite(configuration["DbConnectionString"]));

            services.AddTransient<QueryFactory>(_ =>
            {
                var connection = new SqliteConnection(configuration["DbConnectionString"]);

                var compiler = new SqliteCompiler();

                var factory = new QueryFactory(connection, compiler);

                return factory;
            });

            return services;
        }

        private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            var externalServiceSettings = configuration.GetSection("ExternalServiceSettings").Get<ExternalDataModelProvider.Settings>();

            if (externalServiceSettings != null)
            {
                services.AddSingleton(externalServiceSettings);
            }

            services.AddHttpClient<IExternalDataModelProvider, ExternalDataModelProvider>(client =>
            {
                client.BaseAddress = new Uri(externalServiceSettings.Uri);
                client.Timeout = TimeSpan.FromSeconds(externalServiceSettings.Timeout);
            });

            return services;
        }

        private static void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.IncludeExceptionDetails = (ctx, ex) => false;

            options.Rethrow<NotSupportedException>();

            options.Map<NotFoundException>((httpContext, exception) =>
            {
                return new ProblemDetails()
                {
                    Type = "not-found-error",
                    Title = exception.Message,
                    Status = StatusCodes.Status404NotFound
                };
            });

            options.Map<ValidationException>((httpContext, exception) =>
            {
                var errors = exception.Errors
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Select(x => x.ErrorMessage).ToArray());

                return new ValidationProblemDetails(errors)
                {
                    Title = "One or more validation errors occurred.",
                    Type = "validation-error",
                    Status = StatusCodes.Status400BadRequest
                };
            });
        }
    }
}
