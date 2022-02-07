using PMSvc.Application.Products;
using Shouldly;
using System;
using Xunit;

namespace PMSvc.Test.Application.Products
{
    public class GetProductByIdValidatorTest
    {
        [Fact]
        public void Validate_WithEmptyId_ShouldNotBeValid()
        {
            var sut = new GetProductById.Validator();
            var response = sut.Validate(new GetProductById.Query { Id = Guid.Empty });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithSomeIdValue_ShouldBeValid()
        {
            var sut = new GetProductById.Validator();
            var response = sut.Validate(new GetProductById.Query { Id = Guid.NewGuid() });
            response.IsValid.ShouldBeTrue();
        }
    }
}
