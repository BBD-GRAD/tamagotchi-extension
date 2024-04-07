using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using tamagotchi_pet.Dialogs;

namespace tamagotchi_pet.Services
{
    public class SessionService
    {
        static SessionService()
        {
        }

        public static async void HandleUserSession(string idToken)
        {
            // Authenticate the user
            var (authSuccess, authMessage) = await ApiService.AuthenticateAsync(idToken);
            MessageBox.Show(authMessage);

            if (!authSuccess)
                return;

            // Check for existing pet
            var (hasPet, petMessage, pet) = await ApiService.GetPetAsync(idToken);
            if (!hasPet)
            {
                MessageBox.Show(petMessage);

                // Open the input dialog to get a pet name
                InputDialog inputDialog = new InputDialog();
                if (inputDialog.ShowDialog() == true)
                {
                    string petName = inputDialog.ResponseText; // Get the entered name
                    var (createSuccess, createMessage) = await ApiService.CreatePetAsync(idToken, petName);
                    MessageBox.Show(createMessage);
                }
            }
            else
            {
                MessageBox.Show("Pet already exists: " + pet.PetName);
            }
        }
    }
}