using MediatR;
using Microsoft.AspNetCore.Mvc;
using PMSvc.Application.Reviews;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;
using static PMSvc.Api.Models.Reviews;

namespace PMSvc.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("{productId}/reviews")]
        [SwaggerOperation(Summary = "Register review over a product", 
            Description = "Allows you to register a new review over a product.")]
        [ProducesResponseType(typeof(RegisterReview.Response), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [Produces("application/json")]
        public async Task<IActionResult> Post(Guid productId, [FromBody] CreateRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterReview.Command { ProductId = productId, Id = Guid.NewGuid(), Rating = request.Rating, Comment = request.Comment, CreationDate = DateTimeOffset.UtcNow };
            var response = await _mediator.Send(command, cancellationToken);
            return StatusCode(201, response);
        }

        [HttpPut]
        [Route("{productId}/reviews/{reviewId}")]
        [SwaggerOperation(Summary = "Update review over a product", 
            Description = "Allows you to update a review over a product.")]
        [ProducesResponseType(typeof(UpdateReview.Response), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [Produces("application/json")]
        public async Task<IActionResult> Put(Guid productId, Guid reviewId, [FromBody] UpdateRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateReview.Command { ProductId = productId, Id = reviewId, Rating = request.Rating, Comment = request.Comment, ModificationDate = DateTimeOffset.UtcNow };
            var response = await _mediator.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
