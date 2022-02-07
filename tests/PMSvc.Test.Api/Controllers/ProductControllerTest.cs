using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PMSvc.Api.Controllers;
using PMSvc.Api.Models;
using PMSvc.Application.Products;
using PMSvc.Core.Exceptions;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Api.Controllers
{
    public class ProductControllerTest
    {
        [Fact]
        public async Task Post_WithValues_ShouldBeCreated()
        {
            var expectedResponse = new RegisterProduct.Response { Id = Guid.NewGuid() };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<RegisterProduct.Command>(), 
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
            
            var sut = new ProductController(mediatorMock.Object);
            var request = new Products.CreateRequest { };

            var response = await sut.Post(request, CancellationToken.None);
            
            response.ShouldBeOfType<CreatedAtActionResult>();
            (response as CreatedAtActionResult).Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Post_WithValues_ShouldThrowException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<RegisterProduct.Command>(),
                It.IsAny<CancellationToken>())).Throws(new ValidationException("exception"));

            var sut = new ProductController(mediatorMock.Object);
            var request = new Products.CreateRequest { };

            await sut.Post(request, CancellationToken.None)
                .ShouldThrowAsync<ValidationException>("exception");
        }

        [Fact]
        public async Task Put_WithValues_ShouldBeUpdated()
        {
            var productId = Guid.NewGuid();
            var expectedResponse = new UpdateProduct.Response { Id = productId };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateProduct.Command>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            var sut = new ProductController(mediatorMock.Object);
            var request = new Products.UpdateRequest { };

            var response = await sut.Put(productId, request, CancellationToken.None);

            response.ShouldBeOfType<OkObjectResult>();
            (response as OkObjectResult).Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Put_WithValues_ShouldThrowException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateProduct.Command>(),
                It.IsAny<CancellationToken>())).Throws(new ValidationException("exception"));

            var sut = new ProductController(mediatorMock.Object);
            var request = new Products.UpdateRequest { };

            await sut.Put(Guid.Empty, request, CancellationToken.None)
                .ShouldThrowAsync<ValidationException>("exception");
        }

        [Fact]
        public async Task Put_WithNoExistingProduct_ShouldThrowNotFoundException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<UpdateProduct.Command>(),
                It.IsAny<CancellationToken>())).Throws(new NotFoundException("product doesn't exist"));

            var sut = new ProductController(mediatorMock.Object);
            var request = new Products.UpdateRequest { };

            await sut.Put(Guid.Empty, request, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }

        [Fact]
        public async Task Get_WithValues_ShouldReturnObject()
        {
            var productId = Guid.NewGuid();
            var expectedResponse = new GetProductById.Response { Id = productId };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetProductById.Query>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            var sut = new ProductController(mediatorMock.Object);

            var response = await sut.GetById(productId, CancellationToken.None);

            response.ShouldBeOfType<OkObjectResult>();
            (response as OkObjectResult).Value.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task Get_WithInvalidProductId_ShouldThrowNotFoundException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetProductById.Query>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("product doesn't exist"));

            var sut = new ProductController(mediatorMock.Object);

            await sut.GetById(Guid.NewGuid(), CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }
    }
}
