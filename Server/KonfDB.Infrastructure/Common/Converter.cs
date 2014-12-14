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
using System.Text;

namespace KonfDB.Infrastructure.Common
{
    public class Converter
    {
        /// <summary>
        ///     To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        public static string ToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        ///     Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <returns></returns>
        public static Byte[] ToByte(string input)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(input);
            return byteArray;
        }

        public static string ToString(object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;
            return Convert.ToString(value);
        }

        public static object Enum<T>(string value)
        {
            return (T) System.Enum.Parse(typeof (T), value, true);
        }

        public static object Enum<T>(long value)
        {
            return (T) System.Enum.Parse(typeof (T), value.ToString(), true);
        }

        /// <summary>
        ///     Converts the passed string into a format that can be used when
        ///     passing string data as a stored procedure/sql argument.
        ///     If the passed string is actually a null object, then a string sayaing
        ///     "null" is returned. If the string is not null, then single quotes will be
        ///     added around the data as required when passing a string to a stored procedure
        ///     or sql argument.
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        public static string ToDbString(string data)
        {
            return (data == null) ? "null" : "'" + data.Replace("'", "''") + "'";
        }

        /// <summary>
        ///     Converts the passed numeric data string into a format that can be used
        ///     as an argument to a stored procedure or a sql call.
        ///     If the passed string is a null object, then the string "null" is returned.
        ///     Otherwise the numeric data is returned.
        /// </summary>
        /// <param name="numericData"></param>
        /// <returns></returns>
        public static string ToDbNumber(string data)
        {
            return (data == null) ? "null" : data;
        }
    }
}