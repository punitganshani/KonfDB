#region License and Product Information

// 
//     This file 'ConsoleProxy.cs' is part of KonfDB application - 
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
using System.ServiceModel;
using System.Threading;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDBCF;

namespace KonfDBRC
{
    internal class ConsoleProxy
    {
        [MTAThread]
        internal static void Main(string[] args)
        {
            //Console.Clear();
            Console.WriteLine(@"KonfDBRC : KonfDB Remote Console");
            Console.WriteLine(@"KonfDBRC : Initializing..");
            
            var token = ConnectionFactory.GetUserToken();
            ICommandService commandService = ConnectionFactory.GetInstance();

            if (token == null)
            {
                Console.WriteLine(@"User Authorization failed");
                return;
            }
            Console.WriteLine(@"KonfDBRC : Service connectivity established..");
            Console.WriteLine();

            bool exitLoop = false;

            while (!exitLoop)
            {
                Console.Write(">");
                string line = Console.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    try
                    {

                        var commandOutput = commandService.ExecuteCommand(line, token);

                        if (commandOutput != null)
                        {
                            if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Message)
                                if (commandOutput.Data != null)
                                    Console.WriteLine(commandOutput.Data.ToJson());
                                else
                                    Console.WriteLine(commandOutput.DisplayMessage);
                            else if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Error)
                                Console.WriteLine(commandOutput.DisplayMessage);

                            if (commandOutput.PostAction == CommandOutput.PostCommandAction.ExitApplication)
                            {
                                exitLoop = true;
                            }
                        }
                    }
                    catch (FaultException fexception)
                    {
                        Console.WriteLine(fexception.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }


            Console.WriteLine(@"Thanks for using KonfDBRC. Press any key to exit.");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}