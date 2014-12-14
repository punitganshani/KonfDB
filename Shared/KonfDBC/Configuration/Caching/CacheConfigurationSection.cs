#region License and Product Information

// 
//     This file 'CacheConfigurationSection.cs' is part of KonfDB application - 
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
using Newtonsoft.Json.Converters;

namespace KonfDB.Infrastructure.Configuration.Caching
{
    public class CacheConfigurationSection : ICacheConfiguration
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("mode")]
        [JsonConverter(typeof (StringEnumConverter))]
        public CacheMode Mode { get; set; }

        [JsonProperty("duration")]
        public long DurationInSeconds { get; set; }

        public CacheConfigurationSection()
        {
            Mode = CacheMode.Sliding;
        }
    }
}