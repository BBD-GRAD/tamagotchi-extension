using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace tamagotchi_pet.Utils
{
    public static class TokenStorage
    {
        // Define the path for the data file
        private static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static readonly string myAppFolder = Path.Combine(appDataPath, "Tamagotchi");
        private static readonly string filePath = Path.Combine(myAppFolder, "TokenData.dat");

        static TokenStorage()
        {
            Directory.CreateDirectory(myAppFolder);
        }

        public static void StoreTokens(Dictionary<string, string> tokens)
        {
            string json = JsonConvert.SerializeObject(tokens);
            string encryptedData = EncryptData(json);
            File.WriteAllText(filePath, encryptedData);
        }

        public static Dictionary<string, string> RetrieveTokens()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new Dictionary<string, string>();
                }

                string encryptedData = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(encryptedData))
                {
                    return new Dictionary<string, string>();
                }

                string decryptedJson = DecryptData(encryptedData);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedJson);
            }
            catch (Exception ex)
            {
                Logging.Logger.Debug("Error retrieving tokens from file: " + ex.Message);
                return new Dictionary<string, string>();
            }
        }

        private static string EncryptData(string data)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] encryptedData = ProtectedData.Protect(dataToEncrypt, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        private static string DecryptData(string encryptedData)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedData);
            byte[] decryptedData = ProtectedData.Unprotect(dataToDecrypt, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}