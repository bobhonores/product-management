using System;
using System.Linq;
using PMSvc.Application.Reviews;
using Shouldly;
using Xunit;

namespace PMSvc.Test.Application.Reviews
{
    public class UpdateReviewValidatorTest
    {
        [Fact]
        public void Validate_WithEmptyComand_ShouldNotBeValid()
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command { });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommandWithEmptyId_ShouldNotBeValid()
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command { ProductId = Guid.NewGuid(), Rating = 3 });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommandWithEmptyProductId_ShouldNotBeValid()
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command { Id = Guid.NewGuid(), Rating = 3 });
            response.IsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(10)]
        public void Validate_WithCommandWithInvalidRating_ShouldNotBeValid(int rating)
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Rating = rating
            });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommandWithInvalidImage_ShouldNotBeValid()
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Rating = 5,
                Comment = string.Join(null, Enumerable.Repeat("a", 501))
            });
            response.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_WithCommand_ShouldBeValid()
        {
            var sut = new UpdateReview.Validator();
            var response = sut.Validate(new UpdateReview.Command
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Rating = 5,
                Comment = string.Join(null, Enumerable.Repeat("a", 500))
            });
            response.IsValid.ShouldBeTrue();
        }
    }
}
