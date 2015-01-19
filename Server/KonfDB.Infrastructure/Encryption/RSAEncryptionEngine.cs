using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KonfDB.Infrastructure.Encryption
{
    public class RSAEncryptionEngine : IEncryptionEngine
    {
        private const string Prefix = "+R";

        public string Encrypt(string input, string publicKey, Dictionary<string, object> metadata)
        {
            if (!metadata.ContainsKey("privatekey"))
                return input;

            RSACryptoServiceProvider algorithm = (RSACryptoServiceProvider)metadata["privatekey"];
            byte[] cipherData = algorithm.Encrypt(Encoding.UTF8.GetBytes(input), true);
            return Prefix + Convert.ToBase64String(cipherData);
        }

        public string Decrypt(string input, string privateKey, Dictionary<string, object> metadata)
        {
            if (!metadata.ContainsKey("privatekey"))
                return input;

            if (!input.StartsWith(Prefix))
                return input;

            string newInput = input.Substring(2, input.Length - 2);
            RSACryptoServiceProvider algorithm = (RSACryptoServiceProvider)metadata["privatekey"];
            byte[] plainData = algorithm.Decrypt(Convert.FromBase64String(newInput), true);
            return Encoding.UTF8.GetString(plainData);
        }

        public Tuple<string, string> CreateKeys()
        {
            return null;
        }
    }
}
