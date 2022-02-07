using MediatR;
using Microsoft.AspNetCore.Mvc;
using PMSvc.Application.Products;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using static PMSvc.Api.Models.Products;

namespace PMSvc.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("")]
        [SwaggerOperation(Summary = "Register product info", 
            Description = "Allows you to register new product info.")]
        [ProducesResponseType(typeof(RegisterProduct.Response), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] CreateRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterProduct.Command
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Brand = request.Brand,
                Manufacter = request.Manufacter,
                Model = request.Model,
                Image = request.Image
            };
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction("GetById", new { productId = result.Id }, result);
        }

        [HttpPut]
        [Route("{productId}")]
        [SwaggerOperation(Summary = "Update product info", 
            Description = "Allows you to update existing product info.")]
        [ProducesResponseType(typeof(UpdateProduct.Response), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [Produces("application/json")]
        public async Task<IActionResult> Put(Guid productId, [FromBody] UpdateRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateProduct.Command
            {
                Id = productId,
                Name = request.Name,
                Brand = request.Brand,
                Manufacter = request.Manufacter,
                Model = request.Model,
                Image = request.Image
            };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{productId}")]
        [SwaggerOperation(Summary = "Return product info", 
            Description = "Allows you to obtain the product info. It returns product's reviews and a evaluation made over the product by an external system.")]
        [ProducesResponseType(typeof(UpdateProduct.Response), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [Produces("application/json")]
        public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
        {
            var query = new GetProductById.Query { Id = productId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}