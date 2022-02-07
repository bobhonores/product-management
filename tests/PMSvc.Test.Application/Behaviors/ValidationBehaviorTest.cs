using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using PMSvc.Application.Behaviors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PMSvc.Test.Application.Behaviors
{
    public class ValidationBehaviorTest
    {
        [Fact]
        public async Task Handle_WithNoValidator_ShouldNotThrowException()
        {
            var requestDelegateHandlerMock = new Mock<RequestHandlerDelegate<TestOutput>>();
            var sut = new ValidationBehavior<TestInput, TestOutput>(new List<IValidator<TestInput>>());
            Func<Task> wrappedSut = async () => {
                await sut.Handle(new TestInput(), CancellationToken.None, requestDelegateHandlerMock.Object); };
            await wrappedSut.ShouldNotThrowAsync();
        }

        [Fact]
        public async Task Handle_WithValidatorFailingInput_ShouldThrowValidationException()
        {
            var validationFailures = new[] { new ValidationFailure("MyProperty", "MyProperty has a not valid value") };
            var validatorMock = new Mock<IValidator<TestInput>>();
            validatorMock.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(new ValidationResult(validationFailures));
            var requestDelegateHandlerMock = new Mock<RequestHandlerDelegate<TestOutput>>();
            var sut = new ValidationBehavior<TestInput, TestOutput>(new[] { validatorMock.Object });

            Func<Task> wrappedSut = async () => {
                await sut.Handle(new TestInput(), CancellationToken.None, requestDelegateHandlerMock.Object); };
            await wrappedSut.ShouldThrowAsync<ValidationException>();
        }


        [Fact]
        public async Task Handle_WithSuccessValidator_ShouldNotThrownException()
        {
            var validatorMock = new Mock<IValidator<TestInput>>();
            validatorMock.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                .Returns(new ValidationResult());
            var requestDelegateHandlerMock = new Mock<RequestHandlerDelegate<TestOutput>>();
            var sut = new ValidationBehavior<TestInput, TestOutput>(new[] { validatorMock.Object });

            Func<Task> wrappedSut = async () => {
                await sut.Handle(new TestInput(), CancellationToken.None, requestDelegateHandlerMock.Object); };
            await wrappedSut.ShouldNotThrowAsync();
        }
    }

    public class TestInput : IRequest<TestOutput>
    {
        public string MyProperty { get; set; }
    }

    public class TestOutput
    {

    }
}
