using FluentValidation;
using MediatR;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Application.Reviews
{
    public static class UpdateReview
    {
        public class Command : IRequest<Response>
        {
            public Guid ProductId { get; set; }
            public Guid Id { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public DateTimeOffset ModificationDate { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.ProductId).NotEmpty();
                RuleFor(x => x.Rating).GreaterThanOrEqualTo(0).LessThanOrEqualTo(5);
                RuleFor(x => x.Comment).MaximumLength(500);
            }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public DateTimeOffset ModificationDate { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IProductRepository _productRepository;

            public Handler(IProductRepository productRepository)
            {
                _productRepository = productRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.GetById(request.ProductId, cancellationToken);

                var review = product.Reviews.FirstOrDefault(r => r.Id == request.Id);

                if (review == null)
                {
                    throw new NotFoundException("review doesn't exist");
                }

                review.Rating = request.Rating;

                review.Comment = request.Comment;

                review.ModificationDate = request.ModificationDate;

                await _productRepository.Save(cancellationToken);

                return new Response 
                { 
                    Id = request.Id,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    ModificationDate = request.ModificationDate
                };
            }
        }
    }
}
