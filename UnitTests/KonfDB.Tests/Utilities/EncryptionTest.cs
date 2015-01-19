#region License and Product Information

// 
//     This file 'EncryptionTest.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using System.Security.Cryptography;
using KonfDB.Infrastructure.Encryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KonfDB.Tests.Utilities
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void TestDefaultEncryption()
        {
            const string inputText = "konfdbencryption";
            var keys = EncryptionEngine.Default.CreateKeys();
            string privateKey = keys.Item1;
            string publicKey = keys.Item2;

            var encryptedValue = EncryptionEngine.Default.Encrypt(inputText, publicKey, null);
            var decryptedValue = EncryptionEngine.Default.Decrypt(encryptedValue, privateKey, null);
            Assert.AreEqual(inputText, decryptedValue);
        }

        [TestMethod]
        public void TestMD5Encryption()
        {
            const string inputText = "konfdbencryption";
            var engine = EncryptionEngine.Get<MD5EncryptionEngine>();

            var actual = engine.Encrypt(inputText, null, null);
            Assert.AreEqual("25C58E164132538B20FC4866933DF126", actual);
        }

        [TestMethod]
        public void TestSHAEncryption()
        {
            const string inputText = "konfdbencryption";
            var engine = EncryptionEngine.Get<SHA256Encryption>();

            var actual = engine.Encrypt(inputText, null, null);
            Assert.AreEqual("A4681013792D283BC472487C66449C53A30D4E9A3783952F1CF511F5E40C273A", actual);
        }

        [TestMethod]
        public void TestRSAEncryption()
        {
            const string inputText = "konfdbencryption";
           RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            var engine = EncryptionEngine.Get<RSAEncryptionEngine>();

            var parameters = new Dictionary<string, object> {{"privatekey", csp}};
            var decryptedValue = engine.Encrypt(inputText, null, parameters);
            var actual = engine.Decrypt(decryptedValue, null, parameters);
            Assert.AreEqual(inputText, actual);
        }
    }
}