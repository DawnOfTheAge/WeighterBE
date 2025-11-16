using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeighterBE.Data;
using WeighterBE.Models;

namespace WeighterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightsController : ControllerBase
    {
        #region Data Members

        private readonly ApplicationDbContext _context;

        private readonly ILogger<WeightsController> _logger;

        #endregion

        #region Constructor

        public WeightsController(ApplicationDbContext context, ILogger<WeightsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weight>>> GetWeights()
        {
            try
            {
                _logger.LogInformation("GET Weights");

                return await _context.Weights.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Weights");

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Weight>> GetWeight(int id)
        {
            try
            {
                _logger.LogInformation("GET Weight Id {id}", id);

                var weight = await _context.Weights.FindAsync(id);

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
        public async Task<ActionResult<Weight>> CreateWeight(Weight weight)
        {
            try
            {
                _logger.LogInformation("CREATE Weight");

                _context.Weights.Add(weight);
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
                    if (!_context.Weights.Any(e => e.Id == id))
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

                var weight = await _context.Weights.FindAsync(id);
                if (weight == null)
                {
                    return NotFound();
                }

                _context.Weights.Remove(weight);
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