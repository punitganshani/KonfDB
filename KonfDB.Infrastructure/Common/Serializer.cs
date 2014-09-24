#region License and Product Information

// 
//     This file 'Serializer.cs' is part of KonfDB application - 
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KonfDB.Infrastructure.Common
{
    public static class Serializer
    {
        private static readonly Dictionary<Type, XmlSerializer> _serializer;

        static Serializer()
        {
            _serializer = new Dictionary<Type, XmlSerializer>();
        }

        private static XmlSerializer GetSerializer<T>()
        {
            Type type = typeof (T);
            if (_serializer.ContainsKey(type))
                return _serializer[type];
            XmlSerializer xs = new XmlSerializer(type);
            _serializer[type] = xs;
            return xs;
        }

        /// <summary>
        ///     Serialize an object into an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, Encoding encoding)
        {
            try
            {
                string xmlString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = GetSerializer<T>();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
                xs.Serialize(xmlTextWriter, obj);
                memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
                xmlString = Converter.ToString(memoryStream.ToArray());
                return xmlString;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///     Reconstruct an object from an XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml, Encoding encoding)
        {
            XmlSerializer xs = GetSerializer<T>();
            MemoryStream memoryStream = new MemoryStream(Converter.ToByte(xml));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
            return (T) xs.Deserialize(memoryStream);
        }
    }
}