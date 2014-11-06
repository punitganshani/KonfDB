#region License and Product Information

// 
//     This file 'EnumExtensions.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Configuration.Runtime;
using KonfDB.Infrastructure.WCF;

namespace KonfDB.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt<T>(T value) where T : struct, IConvertible
        {
            if (Enum.GetUnderlyingType(typeof (T)) == typeof (int))
                return Convert.ToInt32(value);

            throw new InvalidCastException("Enum value can not be converted to Int32: " + value);
        }


        public static IList<T> GetValues<T>(this T value) where T : struct, IConvertible
        {
            return
                value.GetType()
                    .GetFields(BindingFlags.Static | BindingFlags.Public)
                    .Select(fi => (T) Enum.Parse(value.GetType(), fi.Name, false))
                    .ToList();
        }

        public static IList<string> GetNames<T>(this T value) where T : struct, IConvertible
        {
            return typeof (T).GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static ServiceType GetWcfServiceType(this EndPointType endPointType)
        {
            if (endPointType == EndPointType.http)
                return ServiceType.BasicHttp;
            if (endPointType == EndPointType.tcp)
                return ServiceType.NetTcp;
            if (endPointType == EndPointType.rest)
                return ServiceType.REST;
            if (endPointType == EndPointType.azurerelay)
                return ServiceType.AzureRelay;

            throw new InvalidDataException("No ServiceType exists for EndPointType: " + endPointType);
        }

        public static bool IsSet(this Enum input, Enum matchTo)
        {
            return (Convert.ToUInt32(input) & Convert.ToUInt32(matchTo)) != 0;
        }
    }
}