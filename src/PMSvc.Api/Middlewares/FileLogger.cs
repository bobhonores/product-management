using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PMSvc.Api.Middlewares
{
    public static class FileLogger
    {
        public class LogOptions
        {
            public string Folder { get; set; }
        }

        public class Middleware
        {
            private readonly RequestDelegate _next;

            private readonly LogOptions _options;

            public Middleware(RequestDelegate next, IOptions<LogOptions> options)
            {
                _next = next;

                _options = options.Value;
            }

            public async Task Invoke(HttpContext context)
            {
                var request = context.Request;
                var stopWatch = new Stopwatch();

                try
                {
                    stopWatch.Start();

                    await _next(context);

                    stopWatch.Stop();
                }
                finally
                {
                    var response = context.Response;
                    var traceId = Activity.Current?.Id.ToString() ?? context.TraceIdentifier;
                    var logMessage = $"TraceId: {traceId} - Duration: {stopWatch.ElapsedMilliseconds} ms - Request: {request.Method} - {request.Path.Value}{request.QueryString} - Response: {response.StatusCode}\n";
                    await File.AppendAllTextAsync(Path.Combine(_options.Folder, $"{DateTimeOffset.UtcNow:yyyyMMdd}.txt"), $"{logMessage}");
                }
            }
        }   
    }
}
