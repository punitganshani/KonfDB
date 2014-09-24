#region License and Product Information

// 
//     This file 'CommandArgs.cs' is part of KonfDB application - 
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KonfDB.Infrastructure.Utilities
{
    public class CommandArgs : IArguments
    {
        // Variables
        private Dictionary<string, string> _parameters;
        private List<string> _args;
        private readonly string _command;

        public string Command
        {
            get { return _command; }
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string parameter]
        {
            get
            {
                if (_parameters.ContainsKey(parameter))
                    return (_parameters[parameter]);
                return null;
            }
        }

        public string this[int parameterNumber]
        {
            get
            {
                if (_parameters.Count > parameterNumber)
                {
                    //if (parameterNumber > 1)
                    //    return _parameters.Skip(parameterNumber - 1).Take(1).First().Value;
                    //else
                    return _parameters.Skip(parameterNumber).Take(1).First().Value;
                }
                return null;
            }
        }

        public CommandArgs(string command)
        {
            _command = command;
            Parse(Regex.Split(command, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"));
        }

        public CommandArgs(IEnumerable<string> args)
        {
            IEnumerable<string> enumerable = args as string[] ?? args.ToArray();
            _command = String.Join(" ", enumerable);
            Parse(enumerable);
        }

        private void Parse(IEnumerable<string> args)
        {
            _parameters = new Dictionary<string, string>();
            _args = args.ToList();

            var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            args.ToList().ForEach(x =>
            {
                var parts = spliter.Split(x, 3).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                switch (parts.Length)
                {
                    case 1:
                        if (_parameters.ContainsKey(parts[0]) == false)
                            _parameters.Add(parts[0], parts[0]);
                        else
                            _parameters[parts[0]] = parts[0];
                        break;
                    case 2:
                        if (_parameters.ContainsKey(parts[0]) == false)
                            _parameters.Add(parts[0], parts[1]);
                        else
                            _parameters[parts[0]] = parts[1];
                        break;
                    case 3:
                        // should ideally not reach here
                        break;
                }
            });
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            _args.ForEach(x => builder.AppendFormat(" {0} ", x));
            return builder.ToString().Trim();
        }

        public bool ContainsKey(string parameterName)
        {
            return _parameters.ContainsKey(parameterName);
        }

        public string[] Parameters
        {
            get { return _parameters.Keys.ToArray(); }
        }

        public void Add(string parameter, string value)
        {
            if (_parameters.ContainsKey(parameter))
                return;
            _parameters.Add(parameter, value);
        }
    }
}