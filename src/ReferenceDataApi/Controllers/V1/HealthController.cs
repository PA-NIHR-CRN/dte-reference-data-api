using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Health.V1.Queries;
using Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ReferenceDataApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class HealthController : Controller
    {
        private readonly IMediator _mediator;

        public HealthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get ethnicity demographics reference data
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="500">Server side error</response>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Dictionary<string, EthnicityResponse>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        [HttpGet("healthconditions")]
        public async Task<IActionResult> GetHealthConditions()
        {
            return Ok(await _mediator.Send(new GetHealthConditionsDataQuery()));
        }
    }
}