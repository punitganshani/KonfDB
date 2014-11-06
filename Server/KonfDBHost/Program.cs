#region License and Product Information

// 
//     This file 'Program.cs' is part of KonfDB application - 
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
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;

namespace KonfDBHost
{
    internal class Program
    {
        [MTAThread]
        internal static void Main(string[] args)
        {
            var backgroundService = new KonfDBH();
            var services = new ServiceBase[] {backgroundService};
            IArguments argsDictionary = new CommandArgs(args);

            if (argsDictionary.ContainsKey("install"))
            {
                ManagedInstallerClass.InstallHelper(new[]
                {
                    Assembly.GetExecutingAssembly().Location
                });
            }
            else if (argsDictionary.ContainsKey("uninstall"))
            {
                ManagedInstallerClass.InstallHelper(new[]
                {
                    "/u",
                    Assembly.GetExecutingAssembly().Location
                });
            }
            else if (argsDictionary.ContainsKey("console")) // console mode
            {
                #region Console

                AppContext.Current.Log.Debug("Running in Console Mode");
                Console.SetWindowPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();

                services.RunService();

                var shutdown = new ManualResetEvent(false);
                var thread = new Thread(() =>
                {
                    while (!shutdown.WaitOne(0))
                    {
                        Thread.Sleep(1000);
                    }
                });
                thread.Start();

                bool exitLoop = false;
                var commandService = backgroundService.ServiceFacade;

                while (!exitLoop)
                {
                    Console.Write(">");
                    string line = Console.ReadLine();

                    if (string.IsNullOrEmpty(line)) continue;

                    var commandOutput = commandService.ExecuteCommand(line, backgroundService.AuthenticationToken);
                    if (commandOutput == null) continue;

                    if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Message)
                        Console.WriteLine(commandOutput.Data != null
                            ? commandOutput.Data.ToJson()
                            : commandOutput.DisplayMessage);
                    else if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Error)
                        Console.WriteLine(commandOutput.DisplayMessage);

                    if (commandOutput.PostAction == CommandOutput.PostCommandAction.ExitApplication)
                    {
                        shutdown.Set();
                        thread.Join();

                        services.StopService();

                        AppContext.Current.Log.Info("Exiting...");

                        Thread.Sleep(500);

                        exitLoop = true;
                    }
                }

                #region SmartConsole

                //ICommandService commandService = new CommandService();

                //var console = new SmartConsole(3, ">")
                //{
                //    GetCommandLists = line => commandService.GetCommandsStartingWith(line),
                //    OnCommandReceived = (consoleInstance, line) =>
                //    {
                //        Debug.WriteLine("Received: " + line);

                //        var commandOutput = commandService.ExecuteCommand(line);
                //        if (commandOutput == null) return;

                //        if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Message)
                //            Console.WriteLine(commandOutput.DisplayMessage);
                //        else if (commandOutput.MessageType == CommandOutput.DisplayMessageType.Error)
                //            Console.WriteLine(commandOutput.DisplayMessage);

                //        if (commandOutput.PostAction == CommandOutput.PostCommandAction.ExitApplication)
                //        {

                //            consoleInstance.Exit();
                //        }
                //    },
                //    OnInit = () => Debug.WriteLine("Initialized"),
                //    OnExit = () =>
                //    {
                //        shutdown.Set();
                //        thread.Join();

                //        services.StopService();

                //        AppContext.Current.Log.Info("Exiting...");
                //        Thread.Sleep(500);
                //    }
                //};

                //console.Host();

                #endregion

                #endregion
            }
            else
            {
                ServiceBase.Run(services);
            }
        }
    }
}