using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WeighterBE.Data;
using WeighterBE.Models;

namespace WeighterBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication for all endpoints
    public class WeightRecordsController(
        ApplicationDbContext context,
        ILogger<WeightRecordsController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<WeightRecordsController> _logger = logger;

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        /// <summary>
        /// Get all weight records for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeightRecord>>> GetWeightRecords(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = _context.WeightRecords
                    .Where(w => w.UserId == userId)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(w => w.RecordedAt >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(w => w.RecordedAt <= endDate.Value);

                var totalRecords = await query.CountAsync();
                var records = await query
                    .OrderByDescending(w => w.RecordedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Append("X-Total-Count", totalRecords.ToString());
                Response.Headers.Append("X-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weight records for user");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Get a specific weight record by ID (only if it belongs to current user)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WeightRecord>> GetWeightRecord(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var record = await _context.WeightRecords
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (record == null)
                {
                    return NotFound(new { message = "Weight record not found" });
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weight record");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Create a new weight record for the current user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WeightRecord>> CreateWeightRecord([FromBody] WeightRecord record)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Override any userId in the request with the authenticated user's ID
                record.UserId = userId;
                record.RecordedAt = record.RecordedAt == default ? DateTime.UtcNow : record.RecordedAt;

                _context.WeightRecords.Add(record);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Weight record created for user {UserId}", userId);

                return CreatedAtAction(
                    nameof(GetWeightRecord),
                    new { id = record.Id },
                    record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weight record");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Update a weight record (only if it belongs to current user)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeightRecord(int id, [FromBody] WeightRecord updatedRecord)
        {
            try
            {
                var userId = GetCurrentUserId();
                var existingRecord = await _context.WeightRecords
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (existingRecord == null)
                {
                    return NotFound(new { message = "Weight record not found" });
                }

                existingRecord.Weight = updatedRecord.Weight;
                existingRecord.Unit = updatedRecord.Unit;
                existingRecord.Notes = updatedRecord.Notes;
                existingRecord.RecordedAt = updatedRecord.RecordedAt;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Weight record {RecordId} updated for user {UserId}", id, userId);

                return Ok(existingRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weight record");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Delete a weight record (only if it belongs to current user)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeightRecord(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var record = await _context.WeightRecords
                    .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

                if (record == null)
                {
                    return NotFound(new { message = "Weight record not found" });
                }

                _context.WeightRecords.Remove(record);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Weight record {RecordId} deleted for user {UserId}", id, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weight record");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Get statistics for the current user's weight records
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetStatistics([FromQuery] DateTime? startDate = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = _context.WeightRecords
                    .Where(w => w.UserId == userId);

                if (startDate.HasValue)
                    query = query.Where(w => w.RecordedAt >= startDate.Value);

                var records = await query.ToListAsync();

                if (records.Count == 0)
                {
                    return Ok(new { message = "No records found" });
                }

                var stats = new
                {
                    totalRecords = records.Count,
                    currentWeight = records.OrderByDescending(w => w.RecordedAt).First().Weight,
                    startWeight = records.OrderBy(w => w.RecordedAt).First().Weight,
                    minWeight = records.Min(w => w.Weight),
                    maxWeight = records.Max(w => w.Weight),
                    avgWeight = records.Average(w => w.Weight),
                    weightChange = records.OrderByDescending(w => w.RecordedAt).First().Weight -
                                   records.OrderBy(w => w.RecordedAt).First().Weight
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        /// <summary>
        /// Admin only: Get all weight records (requires Admin role)
        /// </summary>
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<WeightRecord>>> GetAllWeightRecords()
        {
            try
            {
                var records = await _context.WeightRecords
                    .Include(w => w.User)
                    .OrderByDescending(w => w.RecordedAt)
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all weight records");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }
    }
}
