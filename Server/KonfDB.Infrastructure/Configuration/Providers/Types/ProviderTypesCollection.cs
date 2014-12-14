#region License and Product Information

// 
//     This file 'ProviderTypesCollection.cs' is part of KonfDB application - 
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
using System.Configuration;
using System.Linq;

namespace KonfDB.Infrastructure.Configuration.Providers.Types
{
    [ConfigurationCollection(typeof (ProviderTypeConfiguration))]
    internal class ProviderTypesCollection : List<ProviderTypeConfiguration>
    {
        public ProviderTypeConfiguration this[string name]
        {
            get { return this.FirstOrDefault(x => x.Type == name); }
        }

        public bool IsValid(string type)
        {
            bool isValid = this.Any(x => x.Type == type);

            if (isValid)
            {
                var providerType = this[type];
                var typeOf = Type.GetType(providerType.AssemblyPath);
                return typeOf != null;
            }

            return false;
        }
    }
}