using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PMSvc.Application.Reviews;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using Shouldly;
using Xunit;

namespace PMSvc.Test.Application.Reviews
{
    public class UpdateReviewHandlerTest
    {
        [Fact]
        public async Task Handle_WithValues_ShouldBeUpdated()
        {
            var productId = Guid.NewGuid();
            var expectedResponse = new UpdateReview.Response
            {
                Id = Guid.NewGuid(),
                Rating = 4,
                ModificationDate = DateTimeOffset.UtcNow
            };
            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>
                {
                    new Review { Id = expectedResponse.Id, Rating = 1 }
                }
            };

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var sut = new UpdateReview.Handler(productRepositoryMock.Object);
            var command = new UpdateReview.Command
            {
                ProductId = productId,
                Id = expectedResponse.Id,
                Rating = expectedResponse.Rating,
                ModificationDate = expectedResponse.ModificationDate
            };

            var response = await sut.Handle(command, CancellationToken.None);
            response.Id.ShouldBe(command.Id);
            response.Rating.ShouldBe(command.Rating);
            response.ModificationDate.ShouldBe(command.ModificationDate);
        }

        [Fact]
        public async Task Handle_WithNoExistingProduct_ShouldThrowNotFoundException()
        {
            var productId = Guid.NewGuid();
            
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("product doesn't exist"));

            var sut = new UpdateReview.Handler(productRepositoryMock.Object);
            var command = new UpdateReview.Command
            {
                ProductId = productId,
                Id = Guid.NewGuid(),
                Rating = 4,
                ModificationDate = DateTimeOffset.UtcNow
            };

            await sut.Handle(command, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }


        [Fact]
        public async Task Handle_WithNoExistingReview_ShouldThrowNotFoundException()
        {
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>
                {
                    new Review { Id = Guid.NewGuid(), Rating = 1 }
                }
            };

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var sut = new UpdateReview.Handler(productRepositoryMock.Object);
            var command = new UpdateReview.Command
            {
                ProductId = productId,
                Id = Guid.NewGuid(),
                Rating = 4,
                ModificationDate = DateTimeOffset.UtcNow
            };

            await sut.Handle(command, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("review doesn't exist");
        }

        [Fact]
        public async Task Handle_WithExceptionSaving_ShouldThrowException()
        {
            var productId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Reviews = new List<Review>
                {
                    new Review { Id = reviewId, Rating = 1 }
                }
            };

            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            productRepositoryMock.Setup(x => x.Save(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            var sut = new UpdateReview.Handler(productRepositoryMock.Object);
            var command = new UpdateReview.Command
            {
                ProductId = productId,
                Id = reviewId,
                Rating = 4,
                ModificationDate = DateTimeOffset.UtcNow
            };

            await sut.Handle(command, CancellationToken.None)
                .ShouldThrowAsync<Exception>();
        }
    }
}
