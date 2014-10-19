#region License and Product Information

// 
//     This file 'EnumConverter.cs' is part of KonfDB application - 
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
using System.Globalization;

namespace KonfDB.Engine.Database.ObjectMappers
{
    public class EnumConverter
    {
        public static T Convert<T>(string input) where T : struct
        {
            return (T) Enum.Parse(typeof (T), input);
        }

        public static T Convert<T>(int input) where T : struct
        {
            return (T) Enum.ToObject(typeof (T), input);
        }

        internal static string ConvertToIntString<T>(T input)
        {
            var obj = (int) Enum.ToObject(typeof (T), input);
            return (obj).ToString(CultureInfo.InvariantCulture);
        }

        internal static string ConvertToString<T>(T input)
        {
            return Enum.ToObject(typeof (T), input).ToString();
        }

        internal static int ConvertToInt<T>(T input)
        {
            return (int) Enum.ToObject(typeof (T), input);
        }
    }
}