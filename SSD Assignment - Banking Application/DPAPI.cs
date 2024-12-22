using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace SSD_Assignment___Banking_Application
{
    public class DPAPI
    {
        public static readonly string KeyFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"key.dat");
        public static void GenerateAndStoreKey(string filePath)
        {
            // Check if the key already exists
            if (File.Exists(filePath))
            {
                Console.WriteLine("Key file already exists.");
                return;
            }

            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();

                // Use DPAPI to protect the key and IV
                byte[] protectedKey = ProtectedData.Protect(aes.Key, null, DataProtectionScope.CurrentUser);

                // Write to files
                File.WriteAllBytes(filePath, protectedKey);

                Console.WriteLine("Key and IV have been generated and stored securely.");
            }
        }

        public static void ProtectToFile(byte[] data, string filePath)
        {
            byte[] protectedData = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(filePath, protectedData);
        }

        // Unprotects the data from a file
        public static byte[] UnprotectFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");

            byte[] protectedData = File.ReadAllBytes(filePath);
            byte[] key = ProtectedData.Unprotect(protectedData, null, DataProtectionScope.CurrentUser);
            if (key.Length != 32)
            {
                throw new InvalidOperationException("Key must be 256 bits (32 bytes) long.");
            } else
            {
                return key;
            }
        }
    }
}
