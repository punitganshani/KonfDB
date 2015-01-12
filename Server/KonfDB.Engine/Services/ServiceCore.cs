#region License and Product Information

// 
//     This file 'ServiceCore.cs' is part of KonfDB application - 
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
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Attributes;
using KonfDB.Infrastructure.Commands;
using KonfDB.Infrastructure.Exceptions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Interfaces;
using KonfDB.Infrastructure.Services;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Utilities;

namespace KonfDB.Engine.Services
{
    internal class ServiceCore
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ICommand _helpCommand;
        private readonly ICommand _auditCommand;

        internal ServiceCore()
        {
            _commandFactory = CurrentHostContext.Default.CommandFactory;
            _helpCommand = _commandFactory.Commands.FirstOrDefault(x => x.GetType().Name == "HelpCommand");
            _auditCommand = _commandFactory.Commands.FirstOrDefault(x => x.GetType().Name == "AddAudit");
        }

        internal CommandOutput ExecuteCommand(ServiceRequestContext context)
        {
            var userTokenFromCache = CurrentHostContext.Default.Cache.Get<AuthenticationOutput>(context.Token);

            CommandOutput commandOutput = null;
            try
            {
                var input = new CommandInput(new CommandArgs(context.Command));
                input.Add("Request.SessionId", context.SessionId);
                if (input.Keyword == null)
                    throw new InvalidDataException("Could not parse the command");

                var firstKeyword = input.Keyword.ToLower();
                if (_commandFactory.Commands.Any(x => IsCommand(firstKeyword, x, userTokenFromCache)))
                {
                    var commandObject =
                        _commandFactory.Commands.FirstOrDefault(x => IsCommand(firstKeyword, x, userTokenFromCache));

                    if (userTokenFromCache != null) // enrich with UserId
                        input.AssociateUserId(userTokenFromCache.UserId.GetValueOrDefault(-1));

                    if (RequiresCaching(commandObject))
                    {
                        // cache it
                        commandOutput = CurrentHostContext.Default.Cache.Get(context.Command, () =>
                            ExecuteCommandInternal(input, commandObject));
                    }
                    else
                    {
                        commandOutput = ExecuteCommandInternal(input, commandObject);
                    }

                    AuditExecution(userTokenFromCache, input, commandObject);

                    // Execute all subcommands
                    commandOutput.SubCommands.ForEach(x => ExecuteCommand(new ServiceRequestContext
                    {
                        Command = x,
                        SessionId = context.SessionId,
                        Token = context.Token
                    }));
                }
                else
                {
                    commandOutput = _helpCommand.OnExecute(input);
                }
            }
            catch (DbEntityValidationException e)
            {
                var builder = new StringBuilder();

                foreach (var eve in e.EntityValidationErrors)
                {
                    builder.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State).AppendLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        builder.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage).AppendLine();
                    }
                }

                CurrentHostContext.Default.Log.Error(builder.ToString());
                commandOutput = new CommandOutput
                {
                    MessageType = CommandOutput.DisplayMessageType.Error,
                    DisplayMessage = "Errors: " + builder
                };
            }
            catch (Exception ex)
            {
                CurrentHostContext.Default.Log.Error(ex.GetDetails());
                commandOutput = new CommandOutput
                {
                    MessageType = CommandOutput.DisplayMessageType.Error,
                    DisplayMessage = "Errors: " + ex.GetDetails()
                };
            }

            return commandOutput;
        }

        internal string[] GetCommandsStartingWith(ServiceRequestContext context)
        {
            string lowerCommand = context.Command.ToLowerInvariant();
            return
                _commandFactory.Commands.Where(x => x.Command.ToLowerInvariant().StartsWith(lowerCommand))
                    .Select(x => x.Keyword)
                    .OrderBy(x => x.Length)
                    .ToArray();
        }

        #region Private Methods

        private void AuditExecution(IAuthenticationOutput userTokenFromCache,
    CommandInput input, ICommand commandObject)
        {
            // Audit an AUTH Command
            if (CurrentHostContext.Default.Audit.Enabled)
            {
                var authCommand = commandObject as IAuditCommand;
                if (authCommand != null)
                {
                    var auditRecord = authCommand.GetAuditCommand(input);

                    if (auditRecord != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            var auditInput = new CommandInput(new CommandArgs("AddAudit"));
                            auditInput.Add("record", auditRecord.ToJson());

                            if (userTokenFromCache != null) // enrich with UserId
                                auditInput.AssociateUserId(userTokenFromCache.UserId.GetValueOrDefault(-1));

                            ExecuteCommandInternal(auditInput, _auditCommand);
                        })
                            .ContinueWith(task =>
                                CurrentHostContext.Default.Log.ErrorFormat("Error adding audit record {0}",
                                    task.Exception.GetDetails()), TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
            }
        }

        private CommandOutput ExecuteCommandInternal(CommandInput input, ICommand commandObject)
        {
            CommandOutput commandOutput = null;
            bool executed = false;
            if (commandObject != null)
            {
                if (commandObject.IsValid(input))
                {
                    commandOutput = commandObject.OnExecute(input);
                    executed = true;
                }

                if (!executed)
                {
                    commandOutput = new CommandOutput
                    {
                        PostAction = CommandOutput.PostCommandAction.ShowCommandHelp,
                        DisplayMessage = "One or more parameters were not passed correctly",
                        MessageType = CommandOutput.DisplayMessageType.Error
                    };
                }

                if (commandOutput != null && commandOutput.PostAction == CommandOutput.PostCommandAction.ShowCommandHelp)
                {
                    commandOutput.DisplayMessage += Environment.NewLine
                                                    + "\tUsage:"
                                                    + String.Format("\t{0}", commandObject.Command)
                                                    + Environment.NewLine
                                                    + "\tHelp:"
                                                    + Environment.NewLine
                                                    + Environment.NewLine
                                                    + String.Format("\t{0}", commandObject.Help);
                }
            }

            return commandOutput;
        }

        private static bool RequiresCaching(ICommand commandObject)
        {
            return commandObject.GetCustomAttributesValue<IgnoreCacheAttribute>(false) == null;
        }

        private static bool IsCommand(string firstKeyword, ICommand x, AuthenticationOutput authOutput)
        {
            if (x.Keyword.ToLower().Equals(firstKeyword, StringComparison.InvariantCultureIgnoreCase))
            {
                var authenticationRequired = x.InheritsFrom<IAuthCommand>();
                var isAuthenticated = authOutput != null;

                if ((authenticationRequired && isAuthenticated) || (!authenticationRequired)) return true;

                throw new UnauthorizedUserException(
                    "User has not logged in, or token has expired to executed this command: " + firstKeyword);
            }

            return false;
        }

        #endregion
    }
}