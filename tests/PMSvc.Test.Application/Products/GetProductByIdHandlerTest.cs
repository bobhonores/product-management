using Moq;
using PMSvc.Application.Products;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Application.Products
{
    public class GetProductByIdHandlerTest
    {
        [Fact]
        public async Task Handle_WithProductWithoutReviews_ShouldReturnProduct()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Some product" };
            var productRepositoryMock = new Mock<IProductQueryRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(product);

            var reviewRepositoryMock = new Mock<IReviewQueryRepository>();
            reviewRepositoryMock.Setup(x => x.GetAllByProductId(It.IsAny<Guid>())).ReturnsAsync(new Review[] { });

            var externalDataModel = new DataModel { EventId = $"{Guid.NewGuid()}", Value = 54.23M };
            var externalDataModelProviderMock = new Mock<IExternalDataModelProvider>();
            externalDataModelProviderMock.Setup(x => x.Get(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(externalDataModel);

            var sut = new GetProductById.Handler(productRepositoryMock.Object, reviewRepositoryMock.Object, externalDataModelProviderMock.Object);
            var response = await sut.Handle(new GetProductById.Query { Id = Guid.NewGuid() }, CancellationToken.None);
            response.Id.ShouldBe(product.Id);
            response.Name.ShouldBe(product.Name);
            response.Evaluation.Value.ShouldBe(externalDataModel.Value);
            response.Reviews.ShouldBeEmpty();
        }

        [Fact]
        public async Task Handle_WithProductWithReviews_ShouldReturnProduct()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Some product" };
            var productRepositoryMock = new Mock<IProductQueryRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(product);

            var review = new Review { Id = Guid.NewGuid(), Rating = 3, CreationDate = DateTimeOffset.UtcNow };
            var reviewRepositoryMock = new Mock<IReviewQueryRepository>();
            reviewRepositoryMock.Setup(x => x.GetAllByProductId(It.IsAny<Guid>())).ReturnsAsync(new [] {review });

            var externalDataModel = new DataModel { EventId = $"{Guid.NewGuid()}", Value = 54.23M };
            var externalDataModelProviderMock = new Mock<IExternalDataModelProvider>();
            externalDataModelProviderMock.Setup(x => x.Get(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(externalDataModel);

            var sut = new GetProductById.Handler(productRepositoryMock.Object, reviewRepositoryMock.Object, externalDataModelProviderMock.Object);
            var response = await sut.Handle(new GetProductById.Query { Id = Guid.NewGuid() }, CancellationToken.None);
            response.Id.ShouldBe(product.Id);
            response.Name.ShouldBe(product.Name);
            response.Evaluation.Value.ShouldBe(externalDataModel.Value);
            response.Reviews.ShouldNotBeEmpty();
            response.Reviews.First().Id.ShouldBe(review.Id);
            response.Reviews.First().Rating.ShouldBe(review.Rating);
            response.Reviews.First().CreationDate.ShouldBe(review.CreationDate);
        }

        [Fact]
        public async Task Handle_WithNonExistingProduct_ShouldThrowNotFoundException()
        {
            var productRepositoryMock = new Mock<IProductQueryRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
                .ThrowsAsync(new NotFoundException("product doesn't exist"));

            var reviewRepositoryMock = new Mock<IReviewQueryRepository>();

            var externalDataModelProviderMock = new Mock<IExternalDataModelProvider>();

            var sut = new GetProductById.Handler(productRepositoryMock.Object, reviewRepositoryMock.Object, externalDataModelProviderMock.Object);
            await sut.Handle(new GetProductById.Query { Id = Guid.NewGuid() }, CancellationToken.None)
                .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }

        [Fact]
        public async Task Handle_WithNoDataFromExternalService_ShouldReturnProductWithoutEvaluation()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Some product" };
            var productRepositoryMock = new Mock<IProductQueryRepository>();
            productRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync(product);

            var reviewRepositoryMock = new Mock<IReviewQueryRepository>();

            var externalDataModelProviderMock = new Mock<IExternalDataModelProvider>();
            externalDataModelProviderMock.Setup(x => x.Get(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DataModel { });

            var sut = new GetProductById.Handler(productRepositoryMock.Object, reviewRepositoryMock.Object, externalDataModelProviderMock.Object);
            var response = await sut.Handle(new GetProductById.Query { Id = Guid.NewGuid() }, CancellationToken.None);
            response.Id.ShouldBe(product.Id);
            response.Name.ShouldBe(product.Name);
            response.Evaluation.Value.ShouldBeNull();
        }
    }
}