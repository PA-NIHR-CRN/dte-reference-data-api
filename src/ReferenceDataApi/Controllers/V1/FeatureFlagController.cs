using System.Threading.Tasks;
using Application.FeatureFlags.V1.Queries;
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
    public class FeatureFlagController : Controller
    {
        private readonly IMediator _mediator;

        public FeatureFlagController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get value if feature flag is enabled by service name
        /// </summary>
        /// <response code="200">Ok</response>
        /// <response code="500">Server side error</response>
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(FeatureFlagResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = null)]
        [HttpGet("service/{serviceName}/feature/{featureName}")]
        public async Task<IActionResult> GetFeatureFlag(string serviceName, string featureName)
        {
            return Ok(await _mediator.Send(new GetFeatureFlagQuery(serviceName, featureName)));
        }
    }
}