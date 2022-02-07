using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using PMSvc.Infrastructure;
using Shouldly;
using Xunit;

namespace PMSvc.Test.Infrastructure
{
    public class ProductRepositoryTest
    {
        private DbContextOptions<ProductDbContext> dbContextOptions;

        public ProductRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<ProductDbContext>()
                                .UseInMemoryDatabase(databaseName: "ProductManagementDb")
                                .Options;
        }

        [Fact]
        public async Task Add_WithSomeProduct_ShouldAddProduct()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "name", Model = "model", Brand = "brand" };
            
            var dbContextMock = new ProductDbContext(dbContextOptions);
            var sut = new ProductRepository(dbContextMock);
            await sut.Add(product, CancellationToken.None);
            
            var searchedProduct = await dbContextMock.Products.FindAsync(product.Id);
            searchedProduct.Id.ShouldBe(product.Id);
        }

        [Fact]
        public async Task GetById_WithInexistingProduct_ShouldThrowNotFoundException()
        {
            var dbContextMock = new ProductDbContext(dbContextOptions);
            var sut = new ProductRepository(dbContextMock);
            await sut.GetById(Guid.NewGuid(), CancellationToken.None)
                        .ShouldThrowAsync<NotFoundException>("product doesn't exist");
        }

        [Fact]
        public async Task GetById_WithExistingProduct_ShouldReturnProduct()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "name", Model = "model", Reviews = new List<Review>() };
            var dbContextMock = new ProductDbContext(dbContextOptions);
            await dbContextMock.Products.AddAsync(product);
            await dbContextMock.SaveChangesAsync();

            var sut = new ProductRepository(dbContextMock);
            var result = await sut.GetById(product.Id, CancellationToken.None);
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(product);
        }

        [Fact]
        public async Task Save_WithModifyProduct_ShouldProductBeModified()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "name", Model = "model" };
            var dbContextMock = new ProductDbContext(dbContextOptions);
            await dbContextMock.Products.AddAsync(product);
            await dbContextMock.SaveChangesAsync();

            var existingProduct = await dbContextMock.Products
                                            .FirstOrDefaultAsync(p => p.Id == product.Id);

            var sut = new ProductRepository(dbContextMock);
            existingProduct.Name = "modified name";
            await sut.Save(CancellationToken.None);

            var modifiedProduct = await dbContextMock.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            modifiedProduct.Name.ShouldBe("modified name");
        }
    }
}
