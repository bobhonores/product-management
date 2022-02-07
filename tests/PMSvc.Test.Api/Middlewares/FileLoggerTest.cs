using System;
using System.IO;
using System.Threading.Tasks;
using HttpContextMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using PMSvc.Api.Middlewares;
using Shouldly;
using Xunit;

namespace PMSvc.Test.Api.Middlewares
{
    public class FileLoggerTest : IDisposable
    {
        public void Dispose()
        {
            var filename = $"{DateTimeOffset.UtcNow:yyyyMMdd}.txt";
            
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        [Fact]
        public async Task Invoke_WithValues_ShouldBeTrue()
        {
            var requestDelegateMock = new Mock<RequestDelegate>();
            var httpContextMock = new HttpContextMock
            {
                TraceIdentifier = Guid.NewGuid().ToString()
            };

            var option = Options.Create(new FileLogger.LogOptions { Folder = "" });
            var sut = new FileLogger.Middleware(requestDelegateMock.Object, option);
            await sut.Invoke(httpContextMock);
            File.Exists($"{DateTimeOffset.UtcNow:yyyyMMdd}.txt").ShouldBeTrue();
        }
    }
}
