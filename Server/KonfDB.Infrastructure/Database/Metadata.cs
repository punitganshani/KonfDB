#region License and Product Information

// 
//     This file 'Metadata.cs' is part of KonfDB application - 
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
using System.Linq;

namespace KonfDB.Infrastructure.Database
{
    public static class Metadata
    {
        public static string[] ConfigTables
        {
            get { return SchemaMap["Config"]; }
        }

        public static string[] AccountTables
        {
            get { return SchemaMap["Account"]; }
        }

        public static string[] ReferenceTables
        {
            get { return SchemaMap["Reference"]; }
        }

        public static string[] SettingsTables
        {
            get { return SchemaMap["Settings"]; }
        }

        public static string[] Schemas
        {
            get { return SchemaMap.Keys.Select(x => x).ToArray(); }
        }

        public static string[] ReferenceDataScripts =
        {
            "Data.EnvironmentType",
            "Data.Users",
            "Data.AuditAreas",
            "Data.Options"
        };

        public static readonly Dictionary<String, string[]> SchemaMap;

        static Metadata()
        {
            SchemaMap = new Dictionary<String, string[]>
            {
                {
                    "Reference", new[]
                    {
                        "Country",
                        "City"
                    }
                },
                {
                    "Account", new[]
                    {
                        "Users",
                        "Membership",
                        "Roles"
                    }
                },
                {
                    "Config", new[]
                    {
                        "EnvironmentType",
                        "Suite",
                        "AuditArea",
                        "Audit",
                        "Application",
                        "Category",
                        "Environment",
                        "Parameter",
                        "Region",
                        "Server",
                        "Mapping",
                        "SuiteUsers"
                    }
                },
                {
                    "Settings", new[]
                    {
                        "Options"
                    }
                }
            };
        }
    }
}