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
    public class PetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PetController(ApplicationDbContext context)
        {
            _context = context;
        }

        private UnauthorizedObjectResult UnauthorizedResponse()
        {
            return Unauthorized("User is not authorized to perform this action.");
        }

        // GET: api/Pet
        [HttpGet]
        public async Task<ActionResult<Pet>> GetPet()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var pet = await _context.Pets
                     .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return NotFound("User does not have a pet.");
            }

            return Ok(pet);
        }

        // POST: api/Pet
        [HttpPost]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetDTO petDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var existingPet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existingPet != null)
            {
                return BadRequest("Each user can only have one pet.");
            }

            var pet = new Pet
            {
                UserId = userId,
                PetName = petDTO.PetName,
                Food = 100.0,
                Health = 100.0,
                Water = 100.0,
                Stamina = 100.0,
                XP = 0L
            };

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPet), new { id = pet.PetId }, pet);
        }

        // PUT: api/Pet
        [HttpPut]
        public async Task<IActionResult> UpdatePet([FromBody] UpdatePetDTO petDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pet == null)
            {
                return NotFound("User does not have a pet.");
            }

            var dtoProperties = typeof(UpdatePetDTO).GetProperties();
            foreach (var dtoProperty in dtoProperties)
            {
                var petProperty = typeof(Pet).GetProperty(dtoProperty.Name);
                if (petProperty != null)
                {
                    var dtoValue = dtoProperty.GetValue(petDTO);
                    if (dtoValue != null)
                    {
                        petProperty.SetValue(pet, Convert.ChangeType(dtoValue, petProperty.PropertyType));
                    }
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Pet
        [HttpDelete]
        public async Task<IActionResult> DeletePet()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedResponse();
            }

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pet == null)
            {
                return NotFound("User does not have a pet.");
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}