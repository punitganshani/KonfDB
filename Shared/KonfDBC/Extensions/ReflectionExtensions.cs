#region License and Product Information

// 
//     This file 'ReflectionExtensions.cs' is part of KonfDB application - 
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
using System.IO;
using System.Linq;
using System.Reflection;

namespace KonfDB.Infrastructure.Extensions
{
    public static class ReflectionExtensions
    {
        private static readonly Dictionary<string, bool> InheritanceCache;

        static ReflectionExtensions()
        {
            InheritanceCache = new Dictionary<string, bool>();
        }

        public static bool InheritsFrom<TInterface>(this object input)
        {
            if (input == null) return false;

            //attempt interfaces
            var interfaceType = typeof (TInterface);
            var inputType = input.GetType();
            string key = String.Format(@"{0}{1}", inputType.FullName, interfaceType.FullName);

            if (!InheritanceCache.ContainsKey(key))
                InheritanceCache[key] = inputType.GetInterfaces().Any(x => x == interfaceType);

            return InheritanceCache[key];
        }

        public static bool ImplementsClass<TClass>(this Type inputType)
        {
            if (inputType == null) return false;

            //attempt interfaces
            var interfaceType = typeof(TClass); 
            string key = String.Format(@"{0}{1}", inputType.FullName, interfaceType.FullName);

            if (!InheritanceCache.ContainsKey(key))
                InheritanceCache[key] = inputType.BaseType == interfaceType;

            return InheritanceCache[key];
        }

        public static T GetCustomAttributesValue<T>(this Type input)
        {
            return GetCustomAttributesValue<T>(input, false);
        }

        private static T1 GetCustomAttributesValue<T1>(Type type, bool inherit)
        {
            var attributeType = typeof (T1);
            var attributes = type.GetCustomAttributes(attributeType, inherit);
            if (attributes.Length > 1)
            {
                throw new InvalidOperationException("More than one attribute " + attributeType.Name
                                                    + " found in class " + type.Name);
            }

            if (attributes.Length == 0)
                return default(T1);

            return (T1) attributes[0];
        }

        public static T GetCustomAttributesValue<T>(this object input)
        {
            if (input == null) return default(T);

            return GetCustomAttributesValue<T>(input.GetType(), false);
        }

        public static T GetCustomAttributesValue<T>(this object input, bool inherited)
        {
            if (input == null) return default(T);

            return GetCustomAttributesValue<T>(input.GetType(), inherited);
        }

        public static string GetEmbeddedResource(this Assembly assembly, string provider, string resourceName)
        {
            try
            {
                string transformedName = String.Format("KonfDB.Engine.Assets.Scripts.{0}.{1}.sql", provider,
                    resourceName);
                using (Stream stream = assembly.GetManifestResourceStream(transformedName))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Can not find resource:" + resourceName);
            }
        }
    }
}