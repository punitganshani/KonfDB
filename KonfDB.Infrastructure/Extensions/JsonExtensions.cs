#region License and Product Information

// 
//     This file 'JsonExtensions.cs' is part of KonfDB application - 
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

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KonfDB.Infrastructure.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings Settings;

        static JsonExtensions()
        {
            Settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
            };

            //Settings.Converters.Add(new JavaScriptDateTimeConverter());
        }

        public static T FromJsonToObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static List<T> FromJsonToList<T>(this string value)
        {
            return JsonConvert.DeserializeObject<List<T>>(value, Settings);
        }

        public static string ToJson(this object value)
        {
            var valueAsList = value as IList;
            if (valueAsList != null)
            {
                return ToJson((IList) value);
            }

            try
            {
                //var jObject = JObject.FromObject(value, JsonSerializer.Create(Settings));
                //jObject.AddFirst(new JProperty("Type", value.GetType().Name));
                return JsonConvert.SerializeObject(value, Settings);
            }
            catch
            {
                return value.ToString();
            }
        }

        public static string ToJsonUnIndented(this object value)
        {
            return value.ToJson();
        }

        public static string ToJson<T>(this List<T> value)
        {
            return JsonConvert.SerializeObject(value, typeof (List<T>), Settings);
        }

        public static string ToJson(this IList value)
        {
            return JsonConvert.SerializeObject(value, value.GetType(), Settings);
        }
    }
}