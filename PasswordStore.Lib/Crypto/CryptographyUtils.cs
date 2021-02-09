namespace PasswordStore.Lib.Crypto
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    
    public static class CryptographyUtils
    {
        /// <summary>
        /// Шифрует текстовые данные с использованием заданного текстового ключа с помощью алгоритма AES
        /// </summary>
        /// <param name="masterKey">Текстовый ключ</param>
        /// <param name="data">Шифруемые данные</param>
        /// <returns>Результат шифрования</returns>
        public static string Encrypt(string masterKey, string data)
        {
            using Aes aes = Aes.Create();
            string result = null;
            using (var stream = new MemoryStream())
            {
                var salt = new byte[16];
                using (var rngProvider = new RNGCryptoServiceProvider())
                {
                    rngProvider.GetBytes(salt);
                }
            
                aes.Key = GetAesKey(salt, masterKey);
                byte[] iv = aes.IV;
            
                stream.Write(salt);
                stream.Write(iv);
            
                using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(data);
                }
                result = Convert.ToBase64String(stream.ToArray());
            }

            return result;
        }
        
        /// <summary>
        /// Дешифрует текстовые данные с использованием заданного текстового ключа, защифрованные с помощью алгоритма AES
        /// </summary>
        /// <param name="masterKey">Текстовый ключ</param>
        /// <param name="encryptedData">Зашифрованные данные</param>
        /// <returns>Расшифрованные данные</returns>
        public static string Decrypt(string masterKey, string encryptedData)
        {
            using Aes aes = Aes.Create();
            string result = null;
            using (var stream = new MemoryStream(Convert.FromBase64String(encryptedData)))
            {
                byte[] iv = new byte[aes.IV.Length];
                byte[] salt = new byte[16];

                stream.Read(salt, 0, salt.Length);
                stream.Read(iv, 0, iv.Length);

                var key = GetAesKey(salt, masterKey);

                using (CryptoStream cryptoStream = new CryptoStream(stream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    result = streamReader.ReadToEnd();
                }
            }

            return result;
        }

        /// <summary>
        /// Формирует ключ шифрования AES по стандарту Rfc2898
        /// </summary>
        /// <param name="salt">Соль</param>
        /// <param name="password">Текстовый ключ</param>
        /// <returns>Ключ шифрования AES</returns>
        static byte[] GetAesKey(byte[] salt, string password)
        {
            var bytes = new Rfc2898DeriveBytes(password, salt, 1000);
            return bytes.GetBytes(16);
        }
    }
}