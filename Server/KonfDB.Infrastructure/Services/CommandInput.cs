#region License and Product Information

// 
//     This file 'CommandInput.cs' is part of KonfDB application - 
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
using System.IO;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Infrastructure.Services
{
    public class CommandInput
    {
        public CommandInput(IArguments args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            Args = args;
        }

        private IArguments Args { get; set; }

        public bool HasArgument(string key)
        {
            return Args.ContainsKey(key);
        }

        public string this[string key]
        {
            get { return Args[key]; }
        }

        public string Keyword
        {
            get
            {
                if (Args.Count > 0) return Args[0];
                return null;
            }
        }

        public void AssociateUserId(long userId)
        {
            Args.Add("uid", userId.ToString(CultureInfo.InvariantCulture));
        }

        public void Add(string key, string value)
        {
            if (Args.ContainsKey(key))
                throw new InvalidDataException("The command already has the key: " + key);

            Args.Add(key, value);
        }

        public long GetUserId()
        {
            if (Args.ContainsKey("uid"))
                return long.Parse(Args["uid"]);

            return -1;
        }

        public string Command
        {
            get { return Args.Command; }
        }
    }
}