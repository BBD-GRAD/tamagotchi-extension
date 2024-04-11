using System.Net;

namespace WebTamagotchi.Models
{
    public interface ITamagotchiRepository
    {
        Task<Pet> GetPetByUserId(string userID);

        Task<Uri> CreatePetAsync(Pet pet);

        Task<IEnumerable<Pet>> GetPetsAsync();

        Task<Pet> UpdatePetAsync(Pet pet);

        Task<HttpStatusCode> DeletePetAsync(int petID);

        Task<IEnumerable<User>> GetUsersAsync();
    }
}
