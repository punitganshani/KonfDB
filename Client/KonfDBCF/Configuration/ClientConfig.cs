#region License and Product Information

// 
//     This file 'ClientConfig.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Configuration.Caching;
using KonfDBCF.Configuration.Runtime;

namespace KonfDBCF.Configuration
{
    internal class ClientConfig : ConfigurationSectionGroup
    {
        private new const string Name = "konfDB";
        private static readonly ClientConfig Section;

        [ConfigurationProperty("runtime", IsRequired = true)]
        public RuntimeConfigurationSection Runtime
        {
            get { return (RuntimeConfigurationSection) base.Sections["runtime"]; }
        }

        [ConfigurationProperty("cache", IsRequired = true)]
        public CacheConfigurationSection Caching
        {
            get { return (CacheConfigurationSection) base.Sections["cache"]; }
        }

        static ClientConfig()
        {
            System.Configuration.Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Section = (ClientConfig) config.GetSectionGroup(Name);
        }

        public static ClientConfig ThisSection
        {
            get { return Section; }
        }
    }
}