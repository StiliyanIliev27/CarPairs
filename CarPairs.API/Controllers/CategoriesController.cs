using CarPairs.Core;
using Microsoft.AspNetCore.Mvc;
using CarPairs.Core.Services.Interfaces;

namespace CarPairs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("lookup")]
        public async Task<ActionResult<IEnumerable<SimpleLookupDto>>> GetLookup(CancellationToken cancellationToken)
        {
            var result = await _service.GetLookupAsync(cancellationToken);
            return Ok(result);
        }
    }
}
