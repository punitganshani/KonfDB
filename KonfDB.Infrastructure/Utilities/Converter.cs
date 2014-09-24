#region License and Product Information

// 
//     This file 'Converter.cs' is part of KonfDB application - 
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
using System.Linq;
using System.Text;

namespace KonfDB.Infrastructure.Utilities
{
    public static class Converter
    {
        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x%2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static byte[] GetBytes(this string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        internal static string ToString(byte[] characters)
        {
            var encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        internal static Byte[] ToByte(string input)
        {
            var encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(input);
            return byteArray;
        }

        internal static string ToString(object value, string defaultValue)
        {
            return value == null ? defaultValue : Convert.ToString(value);
        }

        internal static object Enum<T>(string value)
        {
            return (T) System.Enum.Parse(typeof (T), value, true);
        }

        internal static object Enum<T>(long value)
        {
            return (T) System.Enum.Parse(typeof (T), value.ToString(), true);
        }

        internal static string ToDbString(string data)
        {
            return (data == null) ? "null" : "'" + data.Replace("'", "''") + "'";
        }

        internal static string ToDbNumber(string data)
        {
            return data ?? "null";
        }
    }
}