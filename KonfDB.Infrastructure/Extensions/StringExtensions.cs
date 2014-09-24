#region License and Product Information

// 
//     This file 'StringExtensions.cs' is part of KonfDB application - 
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

namespace KonfDB.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private static readonly Random Rand = new Random();
        private const string AllowedCharacters = "bcdfghjklmnpqrstvxz0123456789";

        public static string GetRandom(this string input)
        {
            int numOfCharacters = 10;
            if (!string.IsNullOrEmpty(input) && input.Length >= numOfCharacters)
                numOfCharacters = input.Length;

            int to = AllowedCharacters.Length;
            var qs = new StringBuilder();
            for (int i = 0; i < numOfCharacters; i++)
            {
                qs.Append(AllowedCharacters.Substring(Rand.Next(0, to), 1));
            }
            return qs.ToString();
        }

        public static T ToEnum<T>(this string input) where T : struct, IConvertible
        {
            return (T) Enum.Parse(typeof (T), input, true);
        }
    }
}