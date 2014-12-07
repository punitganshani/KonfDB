#region License and Product Information

// 
//     This file 'RuntimeConfigurationSection.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Enums;

namespace KonfDB.Infrastructure.Configuration.Runtime
{
    internal class RuntimeConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("audit", IsRequired = false)]
        public bool Audit
        {
            get { return (bool) this["audit"]; }
            set { this["audit"] = value; }
        }

        [ConfigurationProperty("params", IsRequired = false)]
        public string Parameters
        {
            get { return (string) this["params"]; }
            set { this["params"] = value; }
        }

        [ConfigurationProperty("logConfigPath", IsRequired = false)]
        public string LogConfigPath
        {
            get { return (string) this["logConfigPath"]; }
            set { this["logConfigPath"] = value; }
        }

        [ConfigurationProperty("securityMode", IsRequired = false)]
        public ServiceSecurityMode ServiceSecurity
        {
            get { return (ServiceSecurityMode) this["securityMode"]; }
            set { this["securityMode"] = value; }
        }

        [ConfigurationProperty("server", IsRequired = false)]
        public ServiceTypeCollection Server
        {
            get { return (ServiceTypeCollection) base["server"]; }
        }

        [ConfigurationProperty("superuser", IsRequired = true)]
        public UserElement SuperUser
        {
            get { return (UserElement) this["superuser"]; }
            set { this["superuser"] = value; }
        }
    }
}