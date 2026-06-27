using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // À implémenter avec une Query dédiée
            return Ok(new List<object>());
        }
    }

}
