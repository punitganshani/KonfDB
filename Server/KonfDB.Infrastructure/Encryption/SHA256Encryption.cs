using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KonfDB.Infrastructure.Encryption
{
    public class SHA256Encryption : IEncryptionEngine
    {
        public string Encrypt(string input, string publicKey)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashValue = mySHA256.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                sb.Append(hashValue[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string Decrypt(string input, string privateKey)
        {
            throw new NotImplementedException();
        }

        public Tuple<string, string> CreateKeys()
        {
            throw new NotImplementedException();
        }
    }
}
