using WebTamagotchi.Models;
using WebTamagotchi.Data;
using System.Net;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.WebRequestMethods;
using Azure;

namespace WebTamagotchi.Data
{
    public class TamagotchiRepository : ITamagotchiRepository
    {
        private readonly HttpClient _client;
        private const string BasePetPath = "/api/Pet";

        public TamagotchiRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Uri> CreatePetAsync(Pet pet)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BasePetPath, pet);
            response.EnsureSuccessStatusCode();
            var dummy = response.IsSuccessStatusCode;
            return response.Headers.Location;
        }

        public async Task<Pet> GetPetAsync()
        {
            Pet PetInfo = null;
            HttpResponseMessage Res = await _client.GetAsync(BasePetPath);
            if (Res.IsSuccessStatusCode)
            {
                var PetResponse = Res.Content.ReadAsStringAsync().Result;
                PetInfo = JsonConvert.DeserializeObject<Pet>(PetResponse);
            }

            return PetInfo;
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{BasePetPath}/{pet.PetId}", pet);
            response.EnsureSuccessStatusCode();

            var PetResponse = response.Content.ReadAsStringAsync().Result;
            pet = JsonConvert.DeserializeObject<Pet>(PetResponse);
            return pet;
        }

        public async Task<HttpStatusCode> DeletePetAsync(int petID)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"{BasePetPath}/{petID}");
            return response.StatusCode;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            List<User> UserInfo = new List<User>();
            HttpResponseMessage Res = await _client.GetAsync("/api/User/Theme");
            if (Res.IsSuccessStatusCode)
            {
                var UserResponse = Res.Content.ReadAsStringAsync().Result;
                UserInfo = JsonConvert.DeserializeObject<List<User>>(UserResponse);
            }

            return UserInfo;
        }

        private const string GoogleClientId = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com";
        private const string GoogleClientSecret = "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";
        private const string GoogleRedirectUri = "https://localhost:32813/api/auth/GoogleCallback";
        private const string clientID = "794918693940-j1kb0o1gi3utki6th2u6nmoc2i40kqbm.apps.googleusercontent.com"; //TODO STORE ENV VARIABLES
        private const string clientSecret = "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";
        private const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";

        public async Task<string> BuildGoogleOAuthUrl()
        {
            string tokenRequestBody = new StringBuilder("https://accounts.google.com/o/oauth2/v2/auth?")
                    .Append("response_type=code")
                    .Append($"&client_id={Uri.EscapeDataString(GoogleClientId)}")
                    .Append($"&redirect_uri={Uri.EscapeDataString(GoogleRedirectUri)}")
                    .Append($"&scope={Uri.EscapeDataString("openid email profile")}")
                    .Append("&access_type=offline")
                    .ToString();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(authorizationEndpoint);
                var dummy = 1;
                //HttpContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                //HttpResponseMessage response = await client.PostAsync(authorizationEndpoint, content);
                //string jsonResponse = await response.Content.ReadAsStringAsync();
                //if (response.IsSuccessStatusCode)
                //{
                //    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                //    return jsonResponse;
                //}
                //else
                //{
                //    Console.WriteLine("RefreshToken: Failed to refresh tokens");
                //}
            }

            return "";
        }

        //[HttpGet("GoogleCallback")]
        //public async Task<IActionResult> GoogleCallback(string code)
        //{
        //    var tokenResponse = await ExchangeCodeForToken(code);
        //    if (!tokenResponse.IsSuccessStatusCode)
        //    {
        //        var errorResponse = await tokenResponse.Content.ReadAsStringAsync();
        //        return BadRequest(errorResponse);
        //    }

        //    var responseContent = await tokenResponse.Content.ReadAsStringAsync();
        //    var jsonResponse = JObject.Parse(responseContent);
        //    var idToken = jsonResponse["id_token"].ToString();
        //    var refreshToken = jsonResponse["refresh_token"].ToString(); // Capture the refresh token

        //    return Ok(new { Jwt = idToken, RefreshToken = refreshToken });
        //}

        private static async Task<HttpResponseMessage> ExchangeCodeForToken(string code)
        {
            var contentString = new StringBuilder()
                .Append("code=").Append(Uri.EscapeDataString(code))
                .Append("&client_id=").Append(Uri.EscapeDataString(GoogleClientId))
                .Append("&client_secret=").Append(Uri.EscapeDataString(GoogleClientSecret))
                .Append("&redirect_uri=").Append(Uri.EscapeDataString(GoogleRedirectUri))
                .Append("&grant_type=authorization_code")
                .ToString();

            using (var client = new HttpClient())
            {
                var content = new StringContent(contentString, Encoding.UTF8, "application/x-www-form-urlencoded");
                return await client.PostAsync("https://oauth2.googleapis.com/token", content);
            }
        }

        public static async Task<bool> RefreshTokensAsync(string idToken, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadToken(idToken) as JwtSecurityToken;
            if (jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(-5))
            {
                string tokenRequestBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
                clientID,
                clientSecret,
                refreshToken
            );

                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(tokenRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.PostAsync(tokenRequestURI, content);
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                        tokenEndpointDecoded.Add("refresh_token", refreshToken);
                        //TokenStorage.StoreTokens(tokenEndpointDecoded);

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("RefreshToken: Failed to refresh tokens");
                    }
                }
            }
            else
            {
                Console.WriteLine("RefreshToken: To early to refresh tokens");
            }
            return false;
        }
    }
}
