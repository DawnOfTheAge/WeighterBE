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
        private readonly ApplicationDbContext _context;

        public WeightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weight>>> GetWeights()
        {
            return await _context.Weights.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Weight>> GetWeight(int id)
        {
            var weight = await _context.Weights.FindAsync(id);

            if (weight == null)
            {
                return NotFound();
            }

            return weight;
        }

        [HttpPost]
        public async Task<ActionResult<Weight>> CreateWeight(Weight weight)
        {
            _context.Weights.Add(weight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeight), new { id = weight.Id }, weight);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeight(int id, Weight weight)
        {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            var weight = await _context.Weights.FindAsync(id);
            if (weight == null)
            {
                return NotFound();
            }

            _context.Weights.Remove(weight);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}