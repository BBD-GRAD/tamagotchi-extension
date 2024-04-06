using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Data;
using TamagotchiAPI.Models;

namespace TamagotchiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID not found in the JWT token.");
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/User/Auth
        [HttpGet("Auth")]
        public async Task<IActionResult> Auth()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("Invalid JWT token: Missing 'sub' or 'email'.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                user = new User
                {
                    UserId = userId,
                    Email = userEmail
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(User), new { id = user.UserId }, user);
            }
            else
            {
                user.Email = userEmail;
                await _context.SaveChangesAsync();
                return Ok(user);
            }
        }
    }
}