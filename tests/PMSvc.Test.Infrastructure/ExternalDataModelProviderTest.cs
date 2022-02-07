using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using PMSvc.Infrastructure;
using Shouldly;
using Xunit;

namespace PMSvc.Test.Infrastructure
{
    public class ExternalDataModelProviderTest
    {
        [Fact]
        public async Task Get_WithOkResponse_ShouldReturnResponse()
        {
            var timestamp = "2022-02-06T23:46:43.852Z";
            var expectedResponse = new Core.DataModel
            {
                EventId = "24l3AfHSUOUgqm5gacnAnCTR2a3",
                DeploymentId = "d_Pas55b8",
                Timestamp = DateTimeOffset.Parse(timestamp),
                Value = 81.76M
            };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"event_id\":\"{expectedResponse.EventId}\",\"deployment_id\":\"{expectedResponse.DeploymentId}\",\"timestamp\":\"{timestamp}\",\"value\":\"{expectedResponse.Value}\"}}")
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object) 
            { 
                BaseAddress = new Uri("http://localhost/") 
            };
            var sut = new ExternalDataModelProvider(httpClient);
            var response = await sut.Get(new Core.Product(), CancellationToken.None);
            response.ShouldNotBeNull();
            response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Get_WithInternalServerError_ShouldReturnEmptyResponse()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent($"{{\"error\":\"Unexpected error\"}}")
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost/") };
            var sut = new ExternalDataModelProvider(httpClient);
            var response = await sut.Get(new Core.Product(), CancellationToken.None);
            response.ShouldNotBeNull();
            response.ShouldBeEquivalentTo(new Core.DataModel());
        }

        [Fact]
        public async Task Get_WithOkDifferentResponse_ShouldReturnEmptyResponse()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"{{\"id\":\"24l3AfHSUOUgqm5gacnAnCTR2a3\",\"modelValue\":\"1002\",\"date\":\"2022-02-06T23:46:43.852Z\"}}")
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };
            var sut = new ExternalDataModelProvider(httpClient);
            var response = await sut.Get(new Core.Product(), CancellationToken.None);
            response.ShouldNotBeNull();
            response.ShouldBeEquivalentTo(new Core.DataModel());
        }
    }
}
