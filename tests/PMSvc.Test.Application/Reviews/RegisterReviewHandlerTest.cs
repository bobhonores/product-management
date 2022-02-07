using Moq;
using PMSvc.Application.Reviews;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Application.Reviews
{
    public class RegisterReviewHandlerTest
    {
        [Fact]
        public async Task Handle_WithCompleteValues_ShouldHaveId()
        {
            var productId = Guid.NewGuid();
            var expectedResponse = new RegisterReview.Response
            {
                Id = Guid.NewGuid(),
                Rating = 3,
                CreationDate = DateTimeOffset.UtcNow
            };
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product { Id = productId, Reviews = new List<Review>() });
            productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var sut = new RegisterReview.Handler(productRepositoryMock.Object);
            var command = new RegisterReview.Command
            {
                ProductId = productId,
                Id = expectedResponse.Id,
                Rating = 3,
                CreationDate = expectedResponse.CreationDate
            };

            var response = await sut.Handle(command, CancellationToken.None);
            response.Id.ShouldNotBe(Guid.Empty);
            response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task Handle_WithNoExistingProduct_ShouldThrowNotFoundException()
        {
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("product doesn't exist"));

            var command = new RegisterReview.Command() { ProductId = Guid.NewGuid() };

            var sut = new RegisterReview.Handler(productRepositoryMock.Object);
            await sut.Handle(command, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithValues_ShouldThrowException()
        {
            var productId = Guid.NewGuid();
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Product { Id = productId, Reviews = new List<Review>() });
            productRepositoryMock.Setup(x => x.Add(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            var command = new RegisterReview.Command { ProductId = productId };

            var sut = new RegisterReview.Handler(productRepositoryMock.Object);
            await sut.Handle(command, CancellationToken.None).ShouldThrowAsync<Exception>();
        }
    }
}
