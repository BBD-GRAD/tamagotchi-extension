using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tamagotchi_pet.Dialogs;
using tamagotchi_pet.Models;
using tamagotchi_pet.Utils;

namespace tamagotchi_pet.Services
{
    internal static class GameService
    {
        internal static async Task<Pet> InitializePetAsync(string idToken)
        {
            if (!string.IsNullOrEmpty(idToken))
            {
                var pet = await ApiService.GetPetAsync(idToken);
                if (pet != null)
                {
                    Logging.Logger.Debug("InitializePetAsync: Pet retrieved: " + pet.PetName);
                    return pet;
                }
                else
                {
                    Logging.Logger.Debug("InitializePetAsync: A pet was not retrieved.");
                    CreatePetDialog inputDialog = new CreatePetDialog();
                    if (inputDialog.ShowDialog() == true)
                    {
                        string petName = inputDialog.ResponseText;
                        Pet createdPet = await ApiService.CreatePetAsync(idToken, petName);
                        Logging.Logger.Debug("InitializePetAsync: Pet created: " + createdPet.PetName);
                        return createdPet;
                    }
                }
            }
            return null;
        }
    }
}