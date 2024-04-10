using WebTamagotchi.Models;
using WebTamagotchi.Helpers;
using WebTamagotchi.Data;
using System.Net;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public async Task<Pet> GetPetByUserId(string userID)
        {
            return new Pet() { Happiness = 10, XP = 2 };
            IEnumerable<Pet> pets = await GetPetsAsync();
            return pets.FirstOrDefault(pet => pet.UserId == userID);
        }

        public async Task<Uri> CreatePetAsync(Pet pet)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(BasePetPath, pet);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        public async Task<IEnumerable<Pet>> GetPetsAsync()
        {
            List<Pet> PetInfo = new List<Pet>();
            HttpResponseMessage Res = await _client.GetAsync(BasePetPath);
            if (Res.IsSuccessStatusCode)
            {
                var PetResponse = Res.Content.ReadAsStringAsync().Result;
                PetInfo = JsonConvert.DeserializeObject<List<Pet>>(PetResponse);
            }

            return PetInfo;

            //List<Pet> PetInfo = null;
            //HttpResponseMessage response = await _client.GetAsync(BasePetPath);
            //if (response.IsSuccessStatusCode)
            //{
            //    PetInfo = await response.ReadContentAsync<List<Pet>>();
            //}
            //return PetInfo;
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{BasePetPath}/{pet.PetId}", pet);
            response.EnsureSuccessStatusCode();

            pet = await response.ReadContentAsync<Pet>();
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
            HttpResponseMessage Res = await _client.GetAsync("/api/User");
            if (Res.IsSuccessStatusCode)
            {
                var UserResponse = Res.Content.ReadAsStringAsync().Result;
                UserInfo = JsonConvert.DeserializeObject<List<User>>(UserResponse);
            }

            return UserInfo;
        }

        //public async Task<IEnumerable<Pet>> GetUserAuthAsync()
        //{
        //    ??? UserAuthInfo = ;
        //    HttpResponseMessage Res = await _client.GetAsync("/api/User/Auth");
        //    if (Res.IsSuccessStatusCode)
        //    {
        //        var UserAuthResponse = Res.Content.ReadAsStringAsync().Result;
        //        UserAuthInfo = JsonConvert.DeserializeObject<???>(UserAuthResponse);
        //    }

        //    return UserAuthInfo;
        //}
    }
}
