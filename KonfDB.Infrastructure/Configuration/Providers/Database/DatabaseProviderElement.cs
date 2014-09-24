#region License and Product Information

// 
//     This file 'DatabaseProviderElement.cs' is part of KonfDB application - 
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

namespace KonfDB.Infrastructure.Configuration.Providers.Database
{
    public class DatabaseProviderElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true, IsKey = true)]
        public string ProviderName
        {
            get { return (string) base["key"]; }
            set { base["key"] = value; }
        }

        [ConfigurationProperty("type")]
        public string Type
        {
            get { return (string) base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("host")]
        public string Host
        {
            get { return (string) base["host"]; }
            set { base["host"] = value; }
        }

        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int) base["port"]; }
            set { base["port"] = value; }
        }

        [ConfigurationProperty("instanceName")]
        public string InstanceName
        {
            get { return (string) base["instanceName"]; }
            set { base["instanceName"] = value; }
        }

        [ConfigurationProperty("username")]
        public string Username
        {
            get { return (string) base["username"]; }
            set { base["username"] = value; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string) base["password"]; }
            set { base["password"] = value; }
        }

        [ConfigurationProperty("location")]
        public string Location
        {
            get { return (string) base["location"]; }
            set { base["location"] = value; }
        }

        public string Transform(string input)
        {
            return input.Replace("$InstanceName$", InstanceName)
                .Replace("$Host$", Host)
                .Replace("$Username$", Username)
                .Replace("$Password$", Password)
                .Replace("$Location$", Location);
        }
    }
}