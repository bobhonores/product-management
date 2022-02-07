using FluentValidation;
using MediatR;
using PMSvc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Application.Products
{
    public static class RegisterProduct
    {
        public class Command : IRequest<Response>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Manufacter { get; set; }
            public string Model { get; set; }
            public string Image { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
                RuleFor(x => x.Brand).NotEmpty().MaximumLength(50);
                RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
                RuleFor(x => x.Image).MaximumLength(150);
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
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IProductRepository _repository;

            public Handler(IProductRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = new Product 
                { 
                    Id = request.Id, 
                    Name = request.Name, 
                    Brand = request.Brand, 
                    Manufacter = request.Manufacter, 
                    Model = request.Model, 
                    Image = request.Image 
                };

                await _repository.Add(product, cancellationToken);

                await _repository.Save(cancellationToken);

                return new Response
                {
                    Id = product.Id,
                    Name = product.Name,
                    Brand = product.Brand,
                    Manufacter = product.Manufacter,
                    Model = product.Model,
                    Image = product.Image
                };
            }
        }
    }
}