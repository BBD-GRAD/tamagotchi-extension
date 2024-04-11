using BlazorTamagotchi.Models;
using System.Net.Http.Headers;
using System.Text;
using System;

using System.Net.Http.Headers;

using System.Net.Http;

using System.Text;

using System.Threading.Tasks;

//using Newtonsoft.Json;

using BlazorTamagotchi.Models;
using System.Text.Json;

//using BlazorTamagotchi.Utils;

//using BlazorTamagotchi.DTOs;

namespace BlazorTamagotchi.Services
{
    public class ApiService
    {
        private readonly HttpClient client = new HttpClient();

        public ApiService()
        {
            client.BaseAddress = new Uri("http://tamagotchi-extension.eu-west-1.elasticbeanstalk.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(bool success, string message)> AuthenticateAsync(string idToken)

        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            try

            {

                HttpResponseMessage response = await client.GetAsync("api/User/Auth");

                if (response.IsSuccessStatusCode)

                {

                    var responseBody = await response.Content.ReadAsStringAsync();

                    return (true, "Logged in successfully!");

                }

                else

                {

                    return (false, "Authentication failed: " + response.ReasonPhrase);

                }

            }

            catch (HttpRequestException e)

            {

                return (false, "Network error: " + e.Message);

            }

        }

        public async Task<Pet> GetPetAsync(string idToken)
        {
            Console.WriteLine(idToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            try
            {
                HttpResponseMessage response = await client.GetAsync("api/Pet");

                if (response.IsSuccessStatusCode)
                {
                    var pet = JsonSerializer.Deserialize<Pet>(await response.Content.ReadAsStringAsync());
                    return pet;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<int> GetThemeAsync(string idToken)

        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            try

            {

                HttpResponseMessage response = await client.GetAsync("api/User/theme");

                if (response.IsSuccessStatusCode)

                {

                    //var theme = JsonSerializer.Deserialize<ThemeDTO>(await response.Content.ReadAsStringAsync());

                    //return theme.ThemeId;

                }

            }

            catch (HttpRequestException e)

            {

                Console.WriteLine($"Network error: {e.Message}");

            }

            return 0;

        }

        //public async Task<bool> PutThemeAsync(string idToken, int themeId)
        //{
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

        //    ThemeDTO themeData = new ThemeDTO
        //    {
        //        ThemeId = themeId
        //    };

        //    try
        //    {
        //        var jsonContent = JsonSerializer.Deserialize(themeData);
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = await client.PutAsync("api/User/theme", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Failed to update theme: {response.StatusCode}");
        //            return false;
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine($"Network error: {e.Message}");
        //        return false;
        //    }
        //}

        public async Task<long> GetHighScoreAsync(string idToken)

        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            try

            {

                HttpResponseMessage response = await client.GetAsync("api/User/highscore");

                if (response.IsSuccessStatusCode)

                {

                    //var highScore = JsonSerializer.Deserialize<HighScoreDTO>(await response.Content.ReadAsStringAsync());

                    //return highScore.Highscore;

                }

            }

            catch (HttpRequestException e)

            {

                Console.WriteLine($"Network error: {e.Message}");

            }

            return 0;

        }

        //public async Task<bool> UpdateHighscoreAsync(string idToken, long xp)

        //{

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

        //    var highScore = new HighScoreDTO

        //    {

        //        Highscore = xp

        //    };

        //    try

        //    {

        //        var jsonContent = JsonConvert.SerializeObject(highScore);

        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = await client.PutAsync("api/User/highscore", content);

        //        if (response.IsSuccessStatusCode)

        //        {

        //            return true;

        //        }

        //        else

        //        {

        //            Console.WriteLine($"Failed to update highscore: {response.StatusCode}");

        //            return false;

        //        }

        //    }

        //    catch (HttpRequestException e)

        //    {

        //        Console.WriteLine($"Network error: {e.Message}");

        //        return false;

        //    }

        //}

        public async Task<bool> DeletePetAsync(string idToken)

        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            try

            {

                HttpResponseMessage response = await client.DeleteAsync($"api/Pet");

                if (response.IsSuccessStatusCode)

                {

                    Console.WriteLine($"Pet deleted successfully");

                    return true;

                }

                else

                {

                    Console.WriteLine($"Failed to delete pet. Status code: {response.StatusCode}");

                }

            }

            catch (HttpRequestException e)

            {

                Console.WriteLine($"Network error during pet deletion: {e.Message}");

            }

            return false;

        }

        public async Task<Pet> CreatePetAsync(string idToken, string petName)

        {

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var petDTO = new

            {

                PetName = petName

            };

            //var jsonContent = JsonSerializer.Deserialize(petDTO);

            //var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try

            {

                //HttpResponseMessage response = await client.PostAsync("api/Pet", contentString);

                //if (response.IsSuccessStatusCode)

                //{

                //    string responseBody = await response.Content.ReadAsStringAsync();

                //    Pet createdPet = JsonSerializer.Deserialize<Pet>(responseBody);

                //    return createdPet;

                //}

            }

            catch (HttpRequestException e)

            {

                Console.WriteLine($"Network error: {e.Message}");

            }

            return null;

        }

        //public async Task<bool> PutPetStatsAsync(string idToken, UpdatePetDTO petStats)
        //{
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
        //    try
        //    {
        //        var jsonContent = JsonConvert.SerializeObject(petStats);
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = await client.PutAsync("api/Pet", content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            Logging.Logger.Debug($"Failed to pet stats: {response.StatusCode}");
        //            return false;
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Logging.Logger.Debug($"Network error: {e.Message}");
        //        return false;
        //    }
        //}
    }
}