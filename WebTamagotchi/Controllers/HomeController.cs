using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using WebTamagotchi.Data;
using WebTamagotchi.Models;

namespace WebTamagotchi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITamagotchiRepository _tamagotchiRepository;

        private ViewModel _memory;

        public HomeController(ITamagotchiRepository tamagotchiRepository, ViewModel memory)
        {
            _tamagotchiRepository = tamagotchiRepository;
            _memory = memory;
        }

        public IActionResult Stats()
        {
            return View(_memory.pet);
        }

        public IActionResult PlayGround()
        {
            _tamagotchiRepository.CreatePetAsync(new Pet() { PetName = "Test"});
            return View(_memory.pet);
        }

        public IActionResult UpdateTheme(string theme)
        {
            ViewBag.Theme = theme;
            //_tamagotchiRepository.updatetheme(theme);

            return View("Playground", _memory.pet);
        }

        //<a class="btn btn-primary" href="@Url.Action("Feed","HomeController")">Click</a>
        public ActionResult Feed()
        {
            if (_memory.currentState.Equals(PetStates.Eating))
            {
                _memory.currentState = PetStates.Happy;
            }
            else
            {
                _memory.currentState = PetStates.Eating;
            }

            StateChecks(false);

            ViewBag.State = _memory.currentState.ToString();

            return View("Playground", _memory.pet);
        }

        //<a class="btn btn-primary" href="@Url.Action("Water","HomeController")">Click</a>
        public ActionResult Water()
        {
            if (_memory.currentState.Equals(PetStates.Drinking))
            {
                _memory.currentState = PetStates.Happy;
            }
            else
            {
                _memory.currentState = PetStates.Drinking;
            }

            StateChecks(false);

            ViewBag.State = _memory.currentState.ToString();

            return View("Playground", _memory.pet);
            
        }

        public ActionResult Rest()
        {
            if (_memory.currentState.Equals(PetStates.Resting))
            {
                _memory.currentState = PetStates.Happy;
            }
            else
            {
                _memory.currentState = PetStates.Resting;
            }

            StateChecks(false);

            ViewBag.State = _memory.currentState.ToString();

            return View("Playground", _memory.pet);
        }

        public IActionResult GetUpdatedModelValues()
        {
            var updatedModel = new
            {
                _memory.pet.Health,
                _memory.pet.XP,
                _memory.pet.Stamina,
                _memory.pet.Food,
                _memory.pet.Water,
                _memory.pet.PetName,
            };
            return Json(updatedModel);
        }

        public async Task<ActionResult> CreatePet(string name)
        {
            _memory.pet.PetName = name;
            _memory.pet.Water = 100;
            _memory.pet.Health = 100;
            _memory.pet.Food = 100;
            _memory.pet.Stamina = 100;
            _memory.pet.XP = 0;

            await _tamagotchiRepository.UpdatePetAsync(_memory.pet);

            ViewBag.CreatePet = "Made";
            return View("Playground", _memory.pet);
        }


        //RedirectToAction("StartGame", "Home", new { UserId = "" });
        public async Task<ActionResult> StartGame(string UserId)
        {
            //pet = await _tamagotchiRepository.GetPetByUserId(UserId);
            if (_memory.pet == null || _memory.pet.Health <= 0)
            {
                _memory.pet = new Pet()
                {
                    UserId = UserId,
                    Water = 100,
                    Health = 100,
                    Food = 100,
                    Stamina = 100,
                    XP = 0
                };
                //_tamagotchiRepository.CreatePetAsync(pet);
            }

            _memory.currentState = PetStates.Happy;
            _memory.gracePeriod = 0;

            _memory.timer = new System.Timers.Timer(/*TimeSpan.FromMinutes(1).TotalMilliseconds*/5000);
            _memory.timer.AutoReset = true;
            _memory.timer.Elapsed += new System.Timers.ElapsedEventHandler(GameLoop);
            _memory.timer.Start();
            
            return View("Playground", _memory.pet);
        }

        //public System.Timers.Timer timer;
        //private Pet pet;
        //private PetStates currentState;
        //private int gracePeriod;
        //private const float healthDrainConst = 100/60;
        public void GameLoop(object sender, ElapsedEventArgs e)
        {
            if (_memory.pet != null)
            {
                _memory.pet.XP += 1;
                _memory.pet.Food -= (100 / 180);
                _memory.pet.Water -= (100 / 60);
                _memory.pet.Stamina -= (100 / 50);

                StateChecks(true);
            }
            else
            {
                ViewBag.CreatePet = "Create";
            }
            PlayGround(); // Change to refresh current view
        }

        private void StateChecks(bool updateValues)
        {
            if (updateValues && _memory.currentState.Equals(PetStates.Hungry) || _memory.currentState.Equals(PetStates.Thirsty) || _memory.currentState.Equals(PetStates.Sleepy))
            {
                if (_memory.gracePeriod <= 0)
                {
                    _memory.pet.Health -= 100/60;
                    if (_memory.pet.Health <= 0)
                    {
                        //_tamagotchiRepository.UpdatePetAsync(pet);

                        //User user = _tamagotchiRepository.GetUserById();
                        //user.Highscore = pet.XP;
                        //_tamagotchiRepository.UpdateUserAsync(user);
                        _memory.pet = null;
                    }
                }
            }
            else if (_memory.currentState.Equals(PetStates.Happy))
            {
                if (_memory.pet.Stamina <= 0)
                {
                    _memory.currentState = PetStates.Sleepy;
                    ViewBag.State = _memory.currentState.ToString();
                }
                else if (_memory.pet.Food <= 0)
                {
                    _memory.currentState = PetStates.Hungry;
                    ViewBag.State = _memory.currentState.ToString();
                }
                else if (_memory.pet.Water <= 0)
                {
                    _memory.currentState = PetStates.Thirsty;
                    ViewBag.State = _memory.currentState.ToString();
                }
                if (!_memory.currentState.Equals(PetStates.Happy) && updateValues)
                {
                    _memory.gracePeriod = 5;
                }
            }
            else if (_memory.currentState.Equals(PetStates.Drinking))
            {
                if (updateValues)
                {
                    _memory.pet.Water += (100 / 5);
                }

                if (_memory.pet.Water >= 100)
                {
                    _memory.pet.Water = 100;

                    _memory.currentState = PetStates.Happy;
                }
            }
            else if (_memory.currentState.Equals(PetStates.Eating))
            {
                if (updateValues)
                {
                    _memory.pet.Food += (100 / 15);
                }

                if (_memory.pet.Food >= 100)
                {
                    _memory.pet.Food = 100;

                    _memory.currentState = PetStates.Happy;
                }
            }
            else if (_memory.currentState.Equals(PetStates.Resting))
            {
                if (updateValues)
                {
                    _memory.pet.Stamina += (100 / 10);
                }

                if (_memory.pet.Stamina >= 100)
                {
                    _memory.pet.Stamina = 100;

                    _memory.currentState = PetStates.Happy;
                }
            }
        }
    }
}
