using MediatR;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.API.Core.Infrstructure.Entities;
using MovieSearch.API.Core.Services.Entities;
using MovieSearch.API.ViewModels;

namespace MovieSearch.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly IMediator mediator;

        public SearchController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("ui")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index()
        {
            var vm= new IndexVM();

            return View(vm);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IndexResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorMessage>), 402)]
        public async Task<IActionResult> Index([FromForm]IndexRequest request)
        {
            var response = await mediator.Send(request);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessages);
        }

        [HttpPost("movies")]
        [ProducesResponseType(typeof(SearchResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorMessage>), 402)]
        public async Task<IActionResult> Movies([FromBody] SearchRequest request)
        {
            var response = await mediator.Send(request);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessages);
        }

        [HttpGet("criteria")]
        [ProducesResponseType(typeof(CriteriaResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorMessage>), 402)]
        public async Task<IActionResult> Criteria([FromQuery]CriteriaRequest request)
        {
            var response = await mediator.Send(request);
            return Ok(response);
        }
    }
}