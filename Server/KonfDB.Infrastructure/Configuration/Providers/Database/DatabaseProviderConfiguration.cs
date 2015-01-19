#region License and Product Information

// 
//     This file 'DatabaseProviderConfiguration.cs' is part of KonfDB application - 
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

using KonfDB.Infrastructure.Configuration.Interfaces;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Configuration.Providers.Database
{
    internal class DatabaseProviderConfiguration : IDatabaseProviderConfiguration
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("instanceName")]
        public string InstanceName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("isEncrypted")]
        public bool IsEncrypted { get; set; }

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