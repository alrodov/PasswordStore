using System;

namespace PasswordStore.CLI
{
    using PasswordStore.Lib.Crypto;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter master key: ");
            var masterKey = Console.ReadLine();
            // var hash = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(masterKey)));
            // if (hash != masterKeyHash)
            // {
            //     Console.WriteLine("Wrong master password");
            //     return;
            // }
            
            Console.Write("Enter working mode (0 to encrypt, 1 to decrypt):");
            var mode = Console.ReadLine();
            switch (mode)
            {
                case "0":
                    Console.Write("Enter value to encrypt: ");
                    var data = Console.ReadLine();
                    Console.Write("Your encrypted data: ");
                    Console.WriteLine(CryptographyUtils.Encrypt(masterKey, data));
                    return;
                case "1":
                    Console.Write("Enter encrypted value: ");
                    data = Console.ReadLine();
                    Console.Write("Your password value: ");
                    Console.WriteLine(CryptographyUtils.Decrypt(masterKey, data));
                    return;
                
            }
        }
    }
}