using Microsoft.AspNetCore.Mvc;
using WeighterBE.Models;

namespace WeighterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightsController : ControllerBase
    {
        private static List<Weight> weights = new();

        /// <summary>
        /// Get all weights
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Weight>> GetWeights()
        {
            return Ok(weights);
        }

        /// <summary>
        /// Get a weight by id
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Weight> GetWeight(int id)
        {
            var weight = weights.FirstOrDefault(w => w.Id == id);
            if (weight == null)
            {
                return NotFound();
            }

            return Ok(weight);
        }

        /// <summary>
        /// Create a new weight
        /// </summary>
        [HttpPost]
        public ActionResult<Weight> CreateWeight(Weight weight)
        {
            if (weight == null)
            {
                return BadRequest();
            }

            weight.Id = weights.Any() ? weights.Max(w => w.Id) + 1 : 1;
            weights.Add(weight);

            return CreatedAtAction(nameof(GetWeight), new { id = weight.Id }, weight);
        }

        /// <summary>
        /// Update an existing weight
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateWeight(int id, Weight weight)
        {
            var existingWeight = weights.FirstOrDefault(w => w.Id == id);
            if (existingWeight == null)
            {
                return NotFound();
            }

            existingWeight.Value = weight.Value;
            existingWeight.WeightAt = weight.WeightAt;

            return NoContent();
        }

        /// <summary>
        /// Delete a weight by id
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteWeight(int id)
        {
            var weight = weights.FirstOrDefault(w => w.Id == id);
            if (weight == null)
            {
                return NotFound();
            }

            weights.Remove(weight);

            return NoContent();
        }
    }
}