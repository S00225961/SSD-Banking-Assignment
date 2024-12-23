using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SSD_Assignment___Banking_Application
{
    public class AES_Encryption
    {

        private static byte[] Key => DPAPI.UnprotectFromFile(DPAPI.KeyFilePath);

        public static (byte[] encryptedData, byte[] iv) Encrypt(string plaintext)
        {
            byte[] encryptedtext;
            byte[] iv;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.GenerateIV();
                iv = aes.IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plaintext);
                        }

                        encryptedtext = memoryStream.ToArray();
                    }
                }
            }
            return (encryptedtext, iv);
        }

        public static string Decrypt(byte[] cipheredtext, byte[] iv)
        {
            string plaintext = String.Empty;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, iv);
                using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            plaintext = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

    }
}
