using Microsoft.AspNetCore.Mvc;

namespace Similarity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly SimilarityService _sim;

        public OffersController(AppDbContext db, SimilarityService sim)
        {
            _db = db;
            _sim = sim;
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id, [FromQuery] int take = 5, [FromQuery] bool allowCrossType = true)
        {
            var item = _db.CatalogItems.Find(id);
            if (item == null) return NotFound();

            var list = _sim.GetSimilar(id, take, allowCrossType);
            var dto = list.Select(x => new {
                x.Item.Id,
                x.Item.Type,
                x.Item.Name,
                x.Item.Description,
                x.Item.CategoryId,
                x.Item.Price,
                Score = x.Score
            });
            return Ok(dto);
        }
    }
}
