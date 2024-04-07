using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace tamagotchi_pet.Utils
{
    public static class SecureTokenStorage
    {
        private static string filePath = "TokenData.dat"; // File to store encrypted token data

        public static void StoreTokens(Dictionary<string, string> tokens)
        {
            // Serialize dictionary to JSON, then encrypt and store
            string json = JsonConvert.SerializeObject(tokens);
            string encryptedData = EncryptData(json);
            File.WriteAllText(filePath, encryptedData);
        }

        public static Dictionary<string, string> RetrieveTokens()
        {
            // Read encrypted data, decrypt, and deserialize back to dictionary
            string encryptedData = File.ReadAllText(filePath);
            string decryptedJson = DecryptData(encryptedData);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedJson);
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