#region License and Product Information

// 
//     This file 'BaseProvider.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Database.Abstracts;
using KonfDB.Infrastructure.Database.StateActions;
using KonfDB.Infrastructure.Shell;
using KonfDB.Infrastructure.Workflow;

namespace KonfDB.Infrastructure.Database.Providers
{
    public abstract class BaseProvider
    {
        public abstract IConfigurationDataStore ConfigurationStore { get; }

        protected StateWorkflow<DatabaseStates> Workflow;

        protected BaseProvider(IDatabaseProviderConfiguration configuration)
        {
            Configuration = configuration;
            Workflow = new StateWorkflow<DatabaseStates>(new StateWorkflowConfig<DatabaseStates>
            {
                AutoExecuteOnStateChanged = true,
                EndState = DatabaseStates.Initialized,
                ThrowExceptions = false
            });

            // NonInitialized -> DB Create
            Workflow.Add<DatabaseCheckAction>(DatabaseStates.NotInitialized, GetDatabaseCheckAction);

            Workflow.Add<DatabaseCreateAction>(DatabaseStates.DatabaseDoesNotExist, GetDatabaseCreateAction);
            Workflow.Add<DatabaseObjectsCheckAction>(DatabaseStates.DatabaseCreated, GetDatabaseObjectsCheckAction);

            Workflow.Add<DatabaseSchemaCreateAction>(DatabaseStates.SchemaMissing, GetDatabaseSchemaCreateAction);
            Workflow.Add<DatabaseObjectsCheckAction>(DatabaseStates.SchemaCreated, GetDatabaseObjectsCheckAction);

            Workflow.Add<DatabaseTableCreateAction>(DatabaseStates.TablesMissing, GetDatabaseTableCreateAction);
            Workflow.Add<DatabaseObjectsCheckAction>(DatabaseStates.TablesCreated, GetDatabaseObjectsCheckAction);

            Workflow.Add<DatabaseReferenceDataAction>(DatabaseStates.ReferencesMissing, GetDatabaseReferenceDataAction);
            Workflow.Add<DatabaseInitializedAction>(DatabaseStates.ReferenceDataPopulated, GetDatabaseInitializedAction);

            Workflow.OnInvalidState =
                (state, dataDictionary, data) => CurrentContext.Default.Log.Error("Unknown DB State:" + state);
            Workflow.OnStateChanged =
                x => CurrentContext.Default.Log.DebugFormat("State changed from {0} to {1}", x.Previous, x.Current);
            Workflow.OnWorkflowEnded = (x, y) => OnInitialized();
        }

        protected IDatabaseProviderConfiguration Configuration;

        public abstract DatabaseCheckAction GetDatabaseCheckAction();
        public abstract DatabaseCreateAction GetDatabaseCreateAction();
        public abstract DatabaseObjectsCheckAction GetDatabaseObjectsCheckAction();
        public abstract DatabaseSchemaCreateAction GetDatabaseSchemaCreateAction();
        public abstract DatabaseTableCreateAction GetDatabaseTableCreateAction();
        public abstract DatabaseReferenceDataAction GetDatabaseReferenceDataAction();
        public abstract DatabaseInitializedAction GetDatabaseInitializedAction();
        public abstract string GetMasterConnectionString();
        public abstract string GetInstanceConnectionString();
        public abstract Dictionary<string, object> GetDataDictionary();

        public abstract void OnInitialized();

        public void Init()
        {
            var dictionary = new Dictionary<string, object>
            {
                {"$MasterConnectionString$", GetMasterConnectionString()},
                {"$InstanceConnectionString$", GetInstanceConnectionString()},
                {"$Configuration$", Configuration},
            };

            var providerSpecificDictionary = GetDataDictionary();
            if (providerSpecificDictionary != null)
            {
                foreach (var keyValuePair in providerSpecificDictionary)
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            var endState = Workflow.ExecuteState(DatabaseStates.NotInitialized, dictionary, null);
            if (endState.Success == false || endState.Errors.Count > 0)
            {
                throw new InvalidOperationException(
                    "Could not initialize DB Provider. Could not reach end-state. States traversed: " + endState.Path);
            }
        }
    }

    public enum DatabaseStates
    {
        UnknownState = 0,
        NotInitialized = 50,
        DatabaseDoesNotExist = 100,
        DatabaseCreated = 200,

        SchemaCreated = 400,
        TablesCreated = 500,
        ReferenceDataPopulated = 600,
        Initialized = 3000,

        DatabaseObjectsMissing = 6666,
        SchemaMissing = 7777,
        TablesMissing = 8888,
        ReferencesMissing = 9999
    }
}