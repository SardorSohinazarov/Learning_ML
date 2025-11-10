using Microsoft.AspNetCore.Mvc;

namespace Similarity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController(SimilarityService sim) : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult Get(int id, [FromQuery] int take = 5, [FromQuery] bool allowCrossType = true)
        {
            var list = sim.GetSimilar(id, take, allowCrossType);
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string search, [FromQuery] int take = 10)
        {
            var list = await sim.SearchAsync(search, take);
            return Ok(list);
        }
    }
}
