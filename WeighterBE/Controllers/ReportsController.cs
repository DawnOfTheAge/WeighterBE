using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeighterBE.Data;
using WeighterBE.Models;
using WeighterBE.Data.DTOs;

namespace WeighterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController(ReportsDbContext context, ILogger<ReportsController> logger) : ControllerBase
    {
        private readonly ReportsDbContext _context = context;
        private readonly ILogger<ReportsController> _logger = logger;

        // GET: api/reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            _logger.LogInformation("Fetching all reports");

            var reports = await _context.Reports.ToListAsync();
            return Ok(reports);
        }

        // GET: api/reports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            _logger.LogInformation("Fetching report {ReportId}", id);

            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                _logger.LogWarning("Report {ReportId} not found", id);
                return NotFound(new { message = $"Report with ID {id} not found" });
            }

            return Ok(report);
        }

        // POST: api/reports
        [HttpPost]
        public async Task<ActionResult<Report>> CreateReport([FromBody] CreateReportDto createDto)
        {
            _logger.LogInformation("Creating new report");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = new Report
            {
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Report {ReportId} created successfully", report.Id);

            return CreatedAtAction(nameof(GetReport), new { id = report.Id }, report);
        }

        // PUT: api/reports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateReportDto updateDto)
        {
            _logger.LogInformation("Updating report {ReportId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                _logger.LogWarning("Report {ReportId} not found for update", id);
                return NotFound(new { message = $"Report with ID {id} not found" });
            }

            report.Description = updateDto.Description;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Report {ReportId} updated successfully", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/reports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            _logger.LogInformation("Deleting report {ReportId}", id);

            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                _logger.LogWarning("Report {ReportId} not found for deletion", id);
                return NotFound(new { message = $"Report with ID {id} not found" });
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Report {ReportId} deleted successfully", id);

            return NoContent();
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.Id == id);
        }
    }
}