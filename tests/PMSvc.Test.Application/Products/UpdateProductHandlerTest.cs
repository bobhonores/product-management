using Moq;
using PMSvc.Application.Products;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Application.Products
{
    public class UpdateProductHandlerTest
    {
        [Fact]
        public async Task Handle_WithCompleteValues_ShouldBeUpdated()
        {
            var product = new Product 
            { 
                Id = Guid.NewGuid(), 
                Name = "original name", 
                Brand = "other brand", 
                Model = "other Model", 
                Image = "other image" 
            };

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var sut = new UpdateProduct.Handler(productRepositoryMock.Object);
            var command = new UpdateProduct.Command 
            { 
                Id = product.Id, 
                Name = "Some name", 
                Brand = "Random brand", 
                Model = "Model", 
                Image = "image" 
            };

            var response = await sut.Handle(command, CancellationToken.None);
            response.Id.ShouldBe(command.Id);
            response.Name.ShouldBe(command.Name);
            response.Brand.ShouldBe(command.Brand);
            response.Model.ShouldBe(command.Model);
            response.Image.ShouldBe(command.Image);
        }

        [Fact]
        public async Task Handle_WithProductDoesntExists_ShouldThrowException()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("product doesn't exist"));

            var command = new UpdateProduct.Command 
            {
                Id = Guid.NewGuid(), 
                Name = "Some name", 
                Brand = "Random brand", 
                Model = "Model", 
                Image = "image" 
            };

            var sut = new UpdateProduct.Handler(productRepositoryMock.Object);
            await sut.Handle(command, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }
    }
}
