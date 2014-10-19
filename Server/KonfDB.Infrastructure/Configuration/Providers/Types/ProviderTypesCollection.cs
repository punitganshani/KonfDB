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
using System.Configuration;
using System.Linq;

namespace KonfDB.Infrastructure.Configuration.Providers.Types
{
    [ConfigurationCollection(typeof (ProviderTypeElement))]
    public class ProviderTypesCollection : ConfigurationElementCollection
    {
        public ProviderTypeElement this[string name]
        {
            get { return (ProviderTypeElement) base.BaseGet(name); }
        }

        public ProviderTypeElement this[int index]
        {
            get { return (ProviderTypeElement) base.BaseGet(index); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderTypeElement) element).Type;
        }

        public bool IsValid(string name)
        {
            var keys = base.BaseGetAllKeys();
            bool isValid = keys.Contains(name);

            if (isValid)
            {
                var providerType = this[name];
                var typeOf = Type.GetType(providerType.AssemblyPath);
                return typeOf != null;
            }

            return isValid;
        }
    }
}