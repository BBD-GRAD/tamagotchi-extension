using BlazorTamagotchi.Models;
using BlazorTamagotchi.Services;
using Microsoft.AspNetCore.Components;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Timers;
using System.Formats.Asn1;

namespace BlazorTamagotchi.Pages
{
    public partial class Home
    {
        private const string GoogleClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
        private const string GoogleClientSecret = "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";
        private const string GoogleRedirectUri = "http://localhost:5245";

        [Inject] NavigationService _navigationService { get; set; }
        [Inject] HttpClient _client { get; set; }

        public string BuildGoogleOAuthUrl()
        {
            return new StringBuilder("https://accounts.google.com/o/oauth2/v2/auth?")
                .Append("response_type=code")
                .Append($"&client_id={Uri.EscapeDataString(GoogleClientId)}")
                .Append($"&redirect_uri={Uri.EscapeDataString(GoogleRedirectUri)}")
                .Append($"&scope={Uri.EscapeDataString("openid email profile")}")
                .Append("&access_type=offline")
                .Append("&prompt=select_account")
                .ToString();
        }

        protected override async Task OnInitializedAsync()
        {
            var uri = new Uri(_navigationService._navigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            string code = query["code"];
            if (!string.IsNullOrEmpty(code))
            {
                await ExchangeCodeForToken(code);
            }
        }

        private async Task ExchangeCodeForToken(string code)
        {
            var requestBody = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", GoogleClientId },
                { "client_secret", GoogleClientSecret },
                { "redirect_uri", GoogleRedirectUri },
                { "grant_type", "authorization_code" }
            };

            var response = await _client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(requestBody));
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                memoryClass.RefreshToken = tokens["refresh_token"];
                memoryClass.IdToken = tokens["id_token"];

                StartGame();
                Console.WriteLine("Authentication successful!");
            }
            else
            {
                Console.WriteLine("Error during token exchange.");
            }
        }

        private async Task StartGame()
        {
            await ApiService.AuthenticateAsync(memoryClass.IdToken);
            pet = await ApiService.GetPetAsync(memoryClass.IdToken);
            if (pet == null)
            {
                CreatePet("MitchTest");
            }
            currentState = PetStates.Happy;
            gracePeriod = 0;

            timer = new System.Timers.Timer(/*TimeSpan.FromMinutes(1).TotalMilliseconds*/1000);
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(GameLoop);
            timer.Start();
        }

        public System.Timers.Timer timer;
        public Pet? pet;
        public long Xp { get { if (pet != null) { return pet.XP; } return 0; } }
        public double Hp { get { if (pet != null) { return pet.Health; } return 0; } }
        public double Food { get { if (pet != null) { return pet.Food; } return 0; } }
        public double Stamina { get { if (pet != null) { return pet.Stamina; } return 0; } }
        public double OurWater { get { if (pet != null) { return pet.Water; } return 0; } }

        public PetStates currentState;
        public double hp = 0;
        public int gracePeriod;
        public const float healthDrainConst = 100 / 60*10; //TODO: update this value

        



        private void GameLoop(object sender, ElapsedEventArgs e)
        {
            if (pet != null)
            {

               pet.XP += 1;

                if (!currentState.Equals(PetStates.Eating))
                {
                    pet.Food -= (double)(100d / 180d);
                    pet.Food = Math.Round(Math.Clamp(pet.Food, 0, 100), 2);
                }

                if (!currentState.Equals(PetStates.Drinking))
                {
                    pet.Water -= (100d / 60d);
                    pet.Water = Math.Round(Math.Clamp(pet.Water, 0, 100), 2);
                }

                if (!currentState.Equals(PetStates.Resting))
                {
                    pet.Stamina -= (100d / 50d);
                    pet.Stamina = Math.Round(Math.Clamp(pet.Stamina, 0, 100), 2);
                }

                StateChecks(true);
            }
            else
            {
                // Popup Create pet
            }
        }
        private async void CheckScore()
        {
            long currentHighScore = await ApiService.GetHighScoreAsync(memoryClass.IdToken);

            if (currentHighScore > pet.XP)
            {
                ApiService.UpdateHighscoreAsync(memoryClass.IdToken, pet.XP);
            }
        }

        public async void CreatePet(string petName)
        {
            await ApiService.CreatePetAsync(memoryClass.IdToken, "petName");
            pet = await ApiService.GetPetAsync(memoryClass.IdToken);
        }

        public void Feed()
        {
            if (currentState.Equals(PetStates.Eating))
            {
                currentState = PetStates.Happy;
            }
            else
            {
                currentState = PetStates.Eating;
            }

            StateChecks(false);
        }

        public void Water()
        {
            if (currentState.Equals(PetStates.Drinking))
            {
                currentState = PetStates.Happy;
            }
            else
            {
                currentState = PetStates.Drinking;
            }

            StateChecks(false);

        }

        public void Rest()
        {
            if (currentState.Equals(PetStates.Resting))
            {
                currentState = PetStates.Happy;
            }
            else
            {
                currentState = PetStates.Resting;
            }

            StateChecks(false);
        }
        private async void StateChecks(bool updateValues)
        {
            if (updateValues && currentState.Equals(PetStates.Hungry) || currentState.Equals(PetStates.Thirsty) || currentState.Equals(PetStates.Sleepy))
            {
                if (gracePeriod <= 0)
                {
                    hp = pet.Health -= 100 / 60;
                    if (pet.Health <= 0)
                    {
                        ApiService.DeletePetAsync(memoryClass.IdToken);
                        CheckScore();
                        currentState = PetStates.Happy;
                        pet = null;
                    }
                }
            }
            else if (currentState.Equals(PetStates.Happy))
            {
                if (pet.Stamina <= 0)
                {
                    currentState = PetStates.Sleepy;
                }
                else if (pet.Food <= 0)
                {
                    currentState = PetStates.Hungry;
                }
                else if (pet.Water <= 0)
                {
                    currentState = PetStates.Thirsty;
                }
                if (!currentState.Equals(PetStates.Happy) && updateValues)
                {
                    gracePeriod = 5;
                }
            }
            else if (currentState.Equals(PetStates.Drinking))
            {
                if (updateValues)
                {
                    pet.Water += (100 / 5);
                }

                if (pet.Water >= 100)
                {
                    pet.Water = 100;

                    currentState = PetStates.Happy;
                }
            }
            else if (currentState.Equals(PetStates.Eating))
            {
                if (updateValues)
                {
                    pet.Food += (100 / 15);
                }

                if (pet.Food >= 100)
                {
                    pet.Food = 100;

                    currentState = PetStates.Happy;
                }
            }
            else if (currentState.Equals(PetStates.Resting))
            {
                if (updateValues)
                {
                    pet.Stamina += (100 / 10);
                }

                if (pet.Stamina >= 100)
                {
                    pet.Stamina = 100;

                    currentState = PetStates.Happy;
                }
            }
            StateHasChanged();
        }

        public enum PetStates
        {
            Hungry,
            Thirsty,
            Sleepy,
            Happy,
            Eating,
            Drinking,
            Resting
        }
    }
}


