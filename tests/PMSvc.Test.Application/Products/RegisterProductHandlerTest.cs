using Moq;
using PMSvc.Application.Products;
using PMSvc.Core;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Application.Products
{
    public class RegisterProductHandlerTest
    {
        [Fact]
        public async Task Handle_WithCompleteValues_ShouldHaveId()
        {
            var productId = Guid.NewGuid();
            var expectedResponse = new RegisterProduct.Response 
            { 
                Id = productId,
                Name = "Product",
                Model = "Model",
                Brand = "MyBrand",
                Manufacter = "Manufacter",
                Image = "image"
            };
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var sut = new RegisterProduct.Handler(productRepositoryMock.Object);
            var command = new RegisterProduct.Command 
            { 
                Id = productId, 
                Name = expectedResponse.Name,
                Model = expectedResponse.Model,
                Brand = expectedResponse.Brand,
                Manufacter = expectedResponse.Manufacter,
                Image = expectedResponse.Image
            };

            var response = await sut.Handle(command, CancellationToken.None);
            response.Id.ShouldNotBe(Guid.Empty);
            response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Handle_WithNoValues_ShouldThrowException()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            var command = new RegisterProduct.Command();

            var sut = new RegisterProduct.Handler(productRepositoryMock.Object);
            await sut.Handle(command, CancellationToken.None).ShouldThrowAsync<Exception>();
        }
    }
}