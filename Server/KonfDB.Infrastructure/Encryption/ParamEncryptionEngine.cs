#region License and Product Information

// 
//     This file 'ParamEncryptionEngine.cs' is part of KonfDB application - 
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
    public class ParamEncryptionEngine : IEncryptionEngine
    {
        public string Encrypt(string input, string publicKey, Dictionary<string, object> metadata = null)
        {
            CspParameters cspParams = new CspParameters {ProviderType = 1};
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

            byte[] plainBytes = Encoding.UTF8.GetBytes(input);
            byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string input, string privateKey, Dictionary<string, object> metadata = null)
        {
            var encryptedBytes = Convert.FromBase64String(input);
            CspParameters cspParams = new CspParameters {ProviderType = 1};
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);
            string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

            return plainText;
        }

        public Tuple<string, string> CreateKeys()
        {
            CspParameters cspParams = new CspParameters {ProviderType = 1};

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024, cspParams);

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return new Tuple<string, string>(privateKey, publicKey);
        }
    }
}