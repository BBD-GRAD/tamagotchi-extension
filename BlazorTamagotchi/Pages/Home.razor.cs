using BlazorTamagotchi.Models;
using BlazorTamagotchi.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorTamagotchi.Pages
{
    public partial class Home
    {
        private const string GoogleClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
        private const string GoogleClientSecret = "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";
        private const string GoogleRedirectUri = "http://localhost:5245";

        [Inject] NavigationService _navigationService { get; set; }
        [Inject] ApiService _apiService { get; set; }
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
                var tokens = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, dynamic>>(responseContent);

                memoryClass.RefreshToken = tokens["refresh_token"].ToString();
                memoryClass.IdToken = tokens["id_token"].ToString();

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
            Pet pet = await _apiService.GetPetAsync(memoryClass.IdToken);

            var dummy = 1;
            _client.BaseAddress = new Uri("http://tamagotchi-extension.eu-west-1.elasticbeanstalk.com/");
        }

        //public async Task<Pet> GetPetAsync()
        //{
        //    HttpResponseMessage response = await _client.PostAsJsonAsync("https://localhost:7163/swagger/index.html/api/Pet", new Pet() { PetName = "Alfred" });
        //    response.EnsureSuccessStatusCode();
        //    Pet PetInfo = null;
        //    HttpResponseMessage Res = await _client.GetAsync("http://tamagotchi-extension.eu-west-1.elasticbeanstalk.com/api/Pet");
        //    if (Res.IsSuccessStatusCode)
        //    {
        //        var PetResponse = Res.Content.ReadAsStringAsync().Result;
        //        PetInfo = JsonSerializer.Deserialize<Pet>(PetResponse);
        //    }

        //    return PetInfo;
        //}
    }
}


