using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using WebTamagotchi.Models;

namespace WebTamagotchi.Controllers
{
    [AllowAnonymous, Route("account")]
    public class AccountController : Controller
    {
        private readonly ITamagotchiRepository _tamagotchiRepository;

        private ViewModel _memory;

        public AccountController(ITamagotchiRepository tamagotchiRepository, ViewModel memory)
        {
            _tamagotchiRepository = tamagotchiRepository;
            _memory = memory;
        }

        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin()
        {
            var dummy = await _tamagotchiRepository.BuildGoogleOAuthUrl();
            var dummy2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(dummy);
            return Json(true);
            return RedirectToAction("StartGame", "Home");
        }
    }
}
