using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PMSvc.Api.Controllers;
using PMSvc.Api.Models;
using PMSvc.Application.Reviews;
using PMSvc.Core.Exceptions;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Api.Controllers
{
    public class ReviewControllerTest
    {
        [Fact]
        public async Task Post_WithValues_ShouldReturnReview()
        {
            var expectedResponse = new RegisterReview.Response { Id = Guid.NewGuid() };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<RegisterReview.Command>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            var sut = new ReviewController(mediatorMock.Object);
            var request = new Reviews.CreateRequest { };

            var response = await sut.Post(Guid.NewGuid(), request, CancellationToken.None);

            response.ShouldBeOfType<ObjectResult>();
            (response as ObjectResult).StatusCode.Value.ShouldBe(201);
            (response as ObjectResult).Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Post_WithNoExistingProduct_ShouldThrowNotFoundException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<RegisterReview.Command>(),
                It.IsAny<CancellationToken>())).Throws(new NotFoundException("product doesn't exists"));

            var sut = new ReviewController(mediatorMock.Object);
            var request = new Reviews.CreateRequest { };

            await sut.Post(Guid.NewGuid(), request, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exists");
        }

        [Fact]
        public async Task Put_WithValues_ShouldReturnReview()
        {
            var expectedResponse = new UpdateReview.Response { Id = Guid.NewGuid() };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateReview.Command>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            var sut = new ReviewController(mediatorMock.Object);
            var request = new Reviews.UpdateRequest { };

            var response = await sut.Put(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None);

            response.ShouldBeOfType<OkObjectResult>();
            (response as OkObjectResult).Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Put_WithNoExistingProduct_ShouldThrowNotFoundException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateReview.Command>(),
                It.IsAny<CancellationToken>())).Throws(new NotFoundException("product doesn't exists"));

            var sut = new ReviewController(mediatorMock.Object);
            var request = new Reviews.UpdateRequest { };

            await sut.Put(Guid.NewGuid(), Guid.NewGuid(), request, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exists");
        }
    }
}
