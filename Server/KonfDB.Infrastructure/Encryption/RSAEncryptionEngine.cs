#region License and Product Information

// 
//     This file 'RSAEncryptionEngine.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
using System.Collections.Generic;
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

            RSACryptoServiceProvider algorithm = (RSACryptoServiceProvider) metadata["privatekey"];
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
            RSACryptoServiceProvider algorithm = (RSACryptoServiceProvider) metadata["privatekey"];
            byte[] plainData = algorithm.Decrypt(Convert.FromBase64String(newInput), true);
            return Encoding.UTF8.GetString(plainData);
        }

        public Tuple<string, string> CreateKeys()
        {
            return null;
        }
    }
}