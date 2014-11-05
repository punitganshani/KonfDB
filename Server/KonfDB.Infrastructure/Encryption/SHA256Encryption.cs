#region License and Product Information

// 
//     This file 'SHA256Encryption.cs' is part of KonfDB application - 
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
    public class SHA256Encryption : IEncryptionEngine
    {
        public string Encrypt(string input, string publicKey)
        {
            SHA256 mySHA256 = SHA256.Create();
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