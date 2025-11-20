using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeighterBE.Data;
using WeighterBE.Models;

namespace WeighterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ApplicationDbContext context, ILogger<UsersController> logger) : ControllerBase
    {
        #region Data Members

        private readonly ApplicationDbContext _context = context;

        private readonly ILogger<UsersController> _logger = logger;

        #endregion
        #region Constructor

        #endregion

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("GET Users");

                return await _context.Users.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting Users");

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation("GET User Id {id}", id);

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Getting User Id {id}", id);

                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            try
            {
                _logger.LogInformation("CREATE User");

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Creating User");

                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            try
            {
                _logger.LogInformation("UPDATE User Id {id}", id);

                if (id != user.Id)
                {
                    return BadRequest();
                }

                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Updating User Id {id}", id);

                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("DELETE User Id {id}", id);

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting User Id {id}", id);

                return StatusCode(500);
            }
        }
    }
}
