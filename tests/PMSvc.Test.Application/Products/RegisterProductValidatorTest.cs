using PMSvc.Application.Products;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace PMSvc.Test.Application.Products
{
    public class RegisterProductValidatorTest
    {

        [Fact]
        public void Validate_WithEmptyComand_ShouldNotBeValid()
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command { });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommandWithEmptyId_ShouldNotBeValid()
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Name = "name",
                Model = "model",
                Brand = "brand",
                Manufacter = "manufacter"
            });
            response.IsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Validate_WithCommandWithInvalidName_ShouldNotBeValid(string name)
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = name,
                Model = "model",
                Brand = "brand",
                Manufacter = "manufacter"
            });
            response.IsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Validate_WithCommandWithInvalidModel_ShouldNotBeValid(string model)
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Model = model,
                Brand = "brand",
                Manufacter = "manufacter"
            });
            response.IsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public void Validate_WithCommandWithInvalidBrand_ShouldNotBeValid(string brand)
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Model = "model",
                Brand = brand,
                Manufacter = "manufacter"
            });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommandWithInvalidImage_ShouldNotBeValid()
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Model = "model",
                Brand = "brand",
                Image = string.Join(null, Enumerable.Repeat("a", 151))
            });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommand_ShouldBeValid()
        {
            var sut = new RegisterProduct.Validator();
            var response = sut.Validate(new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Model = "model",
                Brand = "brand",
                Image = "https://i.picsum.photos/id/866/200/300.jpg?hmac=rcadCENKh4rD6MAp6V_ma-AyWv641M4iiOpe1RyFHeI"
            });
            response.IsValid.ShouldBeTrue();
        }
    }
}
