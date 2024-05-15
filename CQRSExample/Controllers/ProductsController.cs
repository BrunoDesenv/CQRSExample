using CQRSExample.Class;
using CQRSExample.Commands;
using CQRSExample.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRSExample.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Product product)
        {
            {
                await _mediator.Send(new AddProductCommand(product));

                return StatusCode(201);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _mediator.Send(new GetProductsQuery());

            return Ok(products);
        }
    }
}
