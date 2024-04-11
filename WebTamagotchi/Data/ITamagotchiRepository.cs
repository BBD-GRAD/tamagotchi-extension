using System.Net;

namespace WebTamagotchi.Models
{
    public interface ITamagotchiRepository
    {
        Task<Uri> CreatePetAsync(Pet pet);

        Task<Pet> GetPetAsync();

        Task<Pet> UpdatePetAsync(Pet pet);

        Task<HttpStatusCode> DeletePetAsync(int petID);

        Task<string> BuildGoogleOAuthUrl();
    }
}
