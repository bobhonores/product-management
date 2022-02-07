using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PMSvc.Core;
using Review = PMSvc.Application.Products.GetProductById.Response.Review;

namespace PMSvc.Application.Products
{
    public static class GetProductById
    {
        public class Query : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Manufacter { get; set; }
            public string Model { get; set; }
            public string Image { get; set; }
            public DataModel Evaluation { get; set; }
            public IEnumerable<Review> Reviews { get; set; }

            public class DataModel
            {
                public string EventId { get; set; }
                public string DeploymentId { get; set; }
                public DateTimeOffset? Timestamp { get; set; }
                public decimal? Value { get; set; }
            }

            public class Review
            {
                public Guid Id { get; set; }
                public decimal Rating { get; set; }
                public string Comment { get; set; }
                public DateTimeOffset CreationDate { get; set; }
                public DateTimeOffset? ModificationDate { get; set; }
            }
        }
        

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IProductQueryRepository _productRepository;

            private readonly IExternalDataModelProvider _externalModelProvider;

            private readonly IReviewQueryRepository _reviewRepository;

            public Handler(IProductQueryRepository productRepository, IReviewQueryRepository reviewRepository, IExternalDataModelProvider externalModelProvider)
            {

                _productRepository = productRepository;

                _reviewRepository = reviewRepository;

                _externalModelProvider = externalModelProvider;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var product = await _productRepository.GetById(request.Id);

                var externalResponse = await _externalModelProvider.Get(product, cancellationToken);

                var reviews = await _reviewRepository.GetAllByProductId(request.Id);

                return new Response
                {
                    Id = product.Id,
                    Name = product.Name,
                    Model = product.Model,
                    Brand = product.Brand,
                    Manufacter = product.Manufacter,
                    Image = product.Image,
                    Evaluation = new Response.DataModel
                    {
                        EventId = externalResponse?.EventId,
                        DeploymentId = externalResponse?.DeploymentId,
                        Value = externalResponse?.Value,
                        Timestamp = externalResponse?.Timestamp
                    },
                    Reviews = reviews?.Select(r => new Review
                    { 
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreationDate = r.CreationDate,
                        ModificationDate = r.ModificationDate
                    })
                };
            }
        }
    }
}