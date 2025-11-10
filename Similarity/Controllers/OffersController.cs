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
    }
}
