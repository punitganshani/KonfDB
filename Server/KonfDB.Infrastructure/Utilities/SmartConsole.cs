#region License and Product Information

// 
//     This file 'SmartConsole.cs' is part of KonfDB application - 
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
using System.Diagnostics;

namespace KonfDB.Infrastructure.Utilities
{
    public class SmartConsole
    {
        public Action<SmartConsole, string> OnCommandReceived;
        public Action OnInit;
        public Action OnExit;
        public Func<string, string[]> GetCommandLists;

        private readonly List<string> _commands;
        private readonly int _minLength;
        private readonly string _consoleString;
        private bool _exitRequested;

        public SmartConsole(int minLength, string consoleString)
        {
            _minLength = minLength;
            _exitRequested = false;
            _consoleString = consoleString;
            _commands = new List<string>();
        }

        public void Host()
        {
            if (OnCommandReceived == null) throw new InvalidOperationException("No event hooked to process commands");
            if (GetCommandLists == null)
                throw new InvalidOperationException("No event hooked to get possible commands");

            bool exitLoop = false;
            while (!exitLoop || !_exitRequested)
            {
                int commandCounter = 0;

                Console.Write(_consoleString);
                string line = string.Empty;
                int left = Console.CursorLeft;
                int top = Console.CursorTop;

                bool commandSelectedForPassingParameters = false;
                bool tabExitByEnter = false;

                while (true || !_exitRequested)
                {
                    string lastCommand = string.Empty;
                    if (tabExitByEnter) break;
                    var key = Console.ReadKey(true);
                    Debug.WriteLine("InMain.Pressed: " + key.Key);

                    if (string.IsNullOrEmpty(line)
                        && (Console.CursorLeft != left && Console.CursorTop != top)
                        && !string.IsNullOrEmpty(lastCommand))
                        line = lastCommand;

                    if (key.Key == ConsoleKey.Tab)
                    {
                        // Minimum characters required
                        if (line.Length < _minLength)
                            continue;

                        #region Handle Tab Press

                        int counter = 0;
                        var options = GetCommandLists(line);
                        tabExitByEnter = false;

                        while ((options != null && counter < options.Length) || commandSelectedForPassingParameters)
                        {
                            if (tabExitByEnter) break;

                            Console.SetCursorPosition(left, top);
                            if (options == null) continue;

                            Console.Write(options[counter]);

                            var subKey = Console.ReadKey(true);
                            Debug.WriteLine("InTab.Pressed: " + key.Key);
                            if (subKey.Key == ConsoleKey.Spacebar)
                            {
                                commandSelectedForPassingParameters = true;
                                line = options[counter] + subKey.KeyChar;
                                Console.Write(subKey.KeyChar);
                                break;
                            }
                            if (subKey.Key == ConsoleKey.Enter)
                            {
                                tabExitByEnter = true;
                                line = options[counter];
                                break;
                            }
                            if (subKey.Key == ConsoleKey.Backspace)
                            {
                                Debug.WriteLine("InMain.Backspace: " + Console.CursorLeft + " " + left + " " +
                                                Console.CursorTop + " " + top);

                                if (Console.CursorLeft > left + 1 && Console.CursorTop == top)
                                {
                                    Console.Write(subKey.KeyChar);
                                    Console.Write(" ");
                                    Console.Write(subKey.KeyChar);
                                    Debug.WriteLine("Cleared");
                                }

                                line = options[counter];
                                if (line.Length > 1)
                                {
                                    line = line.Remove(line.Length - 1, 1);
                                }

                                break;
                            }
                            if (subKey.Key == ConsoleKey.Tab)
                            {
                                counter++;
                            }
                            else if (subKey.Key == ConsoleKey.Escape)
                            {
                                commandSelectedForPassingParameters = false;
                                break;
                            }
                        }

                        if (!commandSelectedForPassingParameters)
                        {
                            PrintCurrentCommand(line, left, top);
                        }

                        #endregion
                    }
                    else if (key.Key == ConsoleKey.Spacebar)
                    {
                        line += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        commandSelectedForPassingParameters = true;
                        break;
                    }
                    else if (Char.IsControl(key.KeyChar))
                    {
                        #region Backspace, Up, Down, Escape

                        if (key.Key == ConsoleKey.Backspace)
                        {
                            if (line.Length >= 1)
                            {
                                Debug.WriteLine("InMain.Backspace: " + Console.CursorLeft + " " + left + " " +
                                                Console.CursorTop + " " + top);

                                if (Console.CursorLeft > left && Console.CursorTop == top)
                                {
                                    // What is visible should be removed too
                                    line = line.Remove(line.Length - 1, 1);

                                    Console.Write(key.KeyChar);
                                    Console.Write(" ");
                                    Debug.WriteLine("Cleared");
                                    Console.Write(key.KeyChar);
                                }
                            }
                            else
                            {
                                if (Console.CursorLeft <= left && Console.CursorTop == top)
                                    Console.SetCursorPosition(left, top);
                                else
                                    Console.Write(key.KeyChar);
                            }
                        }
                        //else if (key.Key == ConsoleKey.UpArrow)
                        //{
                        //    if (commandCounter == _commands.Count)
                        //    {
                        //        PrintCurrentCommand(line, left, top);
                        //        continue;
                        //    }

                        //    Debug.WriteLine(commandCounter);
                        //    lastCommand = GetLastCommand(-1, commandCounter);
                        //    if (lastCommand != null)
                        //    {
                        //        PrintCurrentCommand(lastCommand, left, top);
                        //        commandCounter++;
                        //    }
                        //    else
                        //        PrintCurrentCommand(line, left, top);
                        //}
                        //else if (key.Key == ConsoleKey.DownArrow)
                        //{
                        //    if (commandCounter == -1)
                        //    {
                        //        PrintCurrentCommand(line, left, top);
                        //        continue;
                        //    }

                        //    Debug.WriteLine(commandCounter);
                        //    lastCommand = GetLastCommand(1, commandCounter);

                        //    if (lastCommand != null)
                        //    {
                        //        PrintCurrentCommand(lastCommand, left, top);
                        //        commandCounter--;
                        //    }
                        //    else
                        //        PrintCurrentCommand(line, left, top);
                        //}
                        //else if (key.Key == ConsoleKey.Escape)
                        //{
                        //    PrintCurrentCommand(line, left, top);
                        //    lastCommand = null;
                        //}
                        else
                        {
                            Console.Write(key.KeyChar);
                        }

                        #endregion
                    }
                    else
                    {
                        line += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }

                if (Console.CursorTop == top)
                    Console.WriteLine();

                if (string.IsNullOrEmpty(line)) continue;

                _commands.Add(line);

                try
                {
                    OnCommandReceived(this, line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing command: " + line);
                }
            }
        }

        private static void PrintCurrentCommand(string line, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write("                                                                      ");
            Console.SetCursorPosition(left, top);
            Console.Write(line);
        }

        private string GetLastCommand(int step, int positionFromLast)
        {
            if (positionFromLast == -1) positionFromLast = 0;

            int totalCommands = _commands.Count;
            if (positionFromLast > totalCommands) return null;

            int position = totalCommands - positionFromLast + step;

            if (position < 0 && totalCommands > 0) position = 0;
            else if (position < 0 && totalCommands == 0) position = -1;
            else if (position >= totalCommands) position = totalCommands - 1;

            return position == -1 ? null : _commands[position];
        }

        public void Exit()
        {
            _exitRequested = true;

            if (OnExit != null)
                OnExit();
        }
    }
}