using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTamagotchi.Data;
using WebTamagotchi.Models;

namespace WebTamagotchi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITamagotchiRepository _tamagotchiRepository;

        private Pet pet;
        public HomeController(ITamagotchiRepository tamagotchiRepository)
        {
            _tamagotchiRepository = tamagotchiRepository;
        }

        public async Task<IActionResult> Stats()
        {
            Pet petModel = await _tamagotchiRepository.GetPetByUserId("");

            return View(petModel);
        }

        [AllowAnonymous]
        public IActionResult PlayGround()
        {
            return View();
        }
    }
}
