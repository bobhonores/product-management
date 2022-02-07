using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using PMSvc.Api.Middlewares;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PMSvc.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFileLogger(this IApplicationBuilder app, FileLogger.LogOptions options)
        {
            if (!Directory.Exists(options.Folder))
            {
                Directory.CreateDirectory(options.Folder);
            }

            return app.UseMiddleware<FileLogger.Middleware>(Options.Create(options));
        }
    }
}
