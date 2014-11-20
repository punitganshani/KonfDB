#region License and Product Information

// 
//     This file 'CertificateProviderElement.cs' is part of KonfDB application - 
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

using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using KonfDB.Infrastructure.Configuration.Interfaces;

namespace KonfDB.Infrastructure.Configuration.Providers.Certificate
{
    public class CertificateProviderElement : ConfigurationElement, ICertificateConfiguration
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string CertificateKey
        {
            get { return (string) base["key"]; }
            set { base["key"] = value; }
        }

        [ConfigurationProperty("storeLocation")]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation) base["storeLocation"]; }
            set { base["storeLocation"] = value; }
        }

        [ConfigurationProperty("storeName")]
        public StoreName StoreName
        {
            get { return (StoreName) base["storeName"]; }
            set { base["storeName"] = value; }
        }

        [ConfigurationProperty("findBy")]
        public X509FindType FindBy
        {
            get { return (X509FindType) base["findBy"]; }
            set { base["findBy"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string) base["value"]; }
            set { base["value"] = value; }
        }
    }
}