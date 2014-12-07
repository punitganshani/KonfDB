#region License and Product Information

// 
//     This file 'CachingResolver.cs' is part of KonfDB application - 
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
using System.Runtime.Serialization;
using System.Xml;

namespace KonfDB.Infrastructure.DataResolvers
{
    public class CachingResolver : DataContractResolver
    {
        private readonly Dictionary<string, int> serializationDictionary;
        private readonly Dictionary<int, string> deserializationDictionary;
        private int serializationIndex;
        private readonly XmlDictionary dic;

        public CachingResolver()
        {
            serializationDictionary = new Dictionary<string, int>();
            deserializationDictionary = new Dictionary<int, string>();
            dic = new XmlDictionary();
        }

        public override bool TryResolveType(Type dataContractType, Type declaredType,
            DataContractResolver knownTypeResolver, out XmlDictionaryString typeName,
            out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
            {
                return false;
            }
            int index;
            if (serializationDictionary.TryGetValue(typeNamespace.Value, out index))
            {
                typeNamespace = dic.Add(index.ToString());
            }
            else
            {
                serializationDictionary.Add(typeNamespace.Value, serializationIndex);
                typeNamespace = dic.Add(serializationIndex++ + "#" + typeNamespace);
            }
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType,
            DataContractResolver knownTypeResolver)
        {
            Type type;
            int deserializationIndex;
            int poundIndex = typeNamespace.IndexOf("#");
            if (poundIndex < 0)
            {
                if (Int32.TryParse(typeNamespace, out deserializationIndex))
                {
                    deserializationDictionary.TryGetValue(deserializationIndex, out typeNamespace);
                }
                type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
            else
            {
                if (Int32.TryParse(typeNamespace.Substring(0, poundIndex), out deserializationIndex))
                {
                    typeNamespace = typeNamespace.Substring(poundIndex + 1, typeNamespace.Length - poundIndex - 1);
                    deserializationDictionary.Add(deserializationIndex, typeNamespace);
                }
                type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
            return type;
        }
    }
}