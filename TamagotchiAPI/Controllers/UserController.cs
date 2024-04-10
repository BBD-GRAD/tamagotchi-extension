using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamagotchiAPI.Data;
using TamagotchiAPI.DTOs;
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
        
        private UnauthorizedObjectResult UnauthorizedResponse()
        {
            return Unauthorized("User is not authorized to perform this action.");
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
                    Email = userEmail,
                    ThemeId = 1,
                    Highscore = 0L,
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCurrentUser), user);
            }
            else
            {
                user.Email = userEmail;
                await _context.SaveChangesAsync();
                return Ok(user);
            }
        }
        
        // GET: api/User/Xp
        [HttpGet("Xp")]
        public async Task<IActionResult> GetXp()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return NotFound("User does not have a pet.");
            }

            return Ok(new XpDTO { XP = pet.XP});
        }

        // PUT: api/User/Xp
        [HttpPut("Xp")]
        public async Task<IActionResult> PutXp([FromBody] XpDTO XpDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            if (XpDTO.XP < 0L)
            {
                return BadRequest("Invalid value for xp!");
            }
            
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return NotFound("User does not have a pet.");
            }

            pet.XP = XpDTO.XP;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // GET: api/User/Theme
        [HttpGet("Theme")]
        public async Task<IActionResult> GetTheme()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(new ThemeDTO { ThemeId = user.ThemeId });
        }
        
        // PUT: api/User/Theme
        [HttpPut("Theme")]
        public async Task<IActionResult> PutTheme([FromBody] ThemeDTO ThemeDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            var theme = await _context.Themes.FindAsync(ThemeDTO.ThemeId);

            if (theme == null)
            {
                return BadRequest($"Theme with Id: {ThemeDTO.ThemeId} is not a valid theme!");
            }

            user.ThemeId = ThemeDTO.ThemeId;

            await _context.SaveChangesAsync();

            return Ok();
        }
        
        // GET: api/User/HighScore
        [HttpGet("HighScore")]
        public async Task<IActionResult> GetHighScore()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(new HighScoreDTO { Highscore = user.Highscore });
        }
    }
}