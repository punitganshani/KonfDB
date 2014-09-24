#region License and Product Information

// 
//     This file 'MD5EncryptionEngine.cs' is part of KonfDB application - 
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
using System.Security.Cryptography;
using System.Text;

namespace KonfDB.Infrastructure.Encryption
{
    public class MD5EncryptionEngine : IEncryptionEngine
    {
        /// <summary>
        ///     Does not use private and public key
        /// </summary>
        /// <param name="input"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public string Encrypt(string input, string publicKey)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public Tuple<string, string> CreateKeys()
        {
            throw new NotImplementedException();
        }

        public string Decrypt(string input, string privateKey)
        {
            throw new NotImplementedException();
        }
    }
}