using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeighterBE.Data;
using WeighterBE.Models;

namespace WeighterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightsController(ApplicationDbContext context, ILogger<WeightsController> logger) : ControllerBase
    {
        #region Data Members

        private readonly ApplicationDbContext _context = context;

        private readonly ILogger<WeightsController> _logger = logger;

        #endregion
        #region Constructor

        #endregion

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeightRecord>>> GetWeights()
        {
            try
            {
                _logger.LogInformation("GET Weights");

                return await _context.WeightRecords.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Weights");

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeightRecord>> GetWeight(int id)
        {
            try
            {
                _logger.LogInformation("GET Weight Id {id}", id);

                var weight = await _context.WeightRecords.FindAsync(id);

                if (weight == null)
                {
                    return NotFound();
                }

                return weight;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Weight Id {id}", id);

                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<WeightRecord>> CreateWeight(WeightRecord weight)
        {
            try
            {
                _logger.LogInformation("CREATE Weight");

                _context.WeightRecords.Add(weight);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetWeight), new { id = weight.Id }, weight);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Creating Weight");

                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeight(int id, Weight weight)
        {
            try
            {
                _logger.LogInformation("UPDATE Weight Id {id}", id);

                if (id != weight.Id)
                {
                    return BadRequest();
                }

                _context.Entry(weight).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.WeightRecords.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Updating Weight Id {id}", id);

                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            try
            {
                _logger.LogInformation("DELETE Weight Id {id}", id);

                var weight = await _context.WeightRecords.FindAsync(id);
                if (weight == null)
                {
                    return NotFound();
                }

                _context.WeightRecords.Remove(weight);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting Weight Id {id}", id);

                return StatusCode(500);
            }
        }
    }
}