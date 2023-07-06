using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.API.Core.Infrstructure.Entities;
using MovieSearch.API.Core.Services.Entities;

namespace MovieSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator mediator;

        public SearchController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IndexResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorMessage>), 402)]
        public async Task<IActionResult> Index([FromForm]IndexRequest request)
        {
            var response = await mediator.Send(request);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessages);
        }
    }
}