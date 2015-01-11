#region License and Product Information

// 
//     This file 'MsSqlProvider.cs' is part of KonfDB application - 
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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Reflection;
using KonfDB.Engine.Database.Stores;
using KonfDB.Infrastructure.Configuration.Interfaces;
using KonfDB.Infrastructure.Database.Abstracts;
using KonfDB.Infrastructure.Database.Providers;
using KonfDB.Infrastructure.Database.StateActions;

namespace KonfDB.Engine.Database.Providers.MsSql
{
    [Export(typeof (BaseProvider))]
    public class MsSqlProvider : BaseProvider
    {
        public MsSqlProvider(IDatabaseProviderConfiguration configuration)
            : base(configuration)
        {
        }

        private IConfigurationDataStore _configurationStore;

        public override IConfigurationDataStore ConfigurationStore
        {
            get { return _configurationStore; }
        }

        public override string GetMasterConnectionString()
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = Configuration.Host,
                InitialCatalog = "master",
                IntegratedSecurity = false,
                UserID = Configuration.Username,
                Password = Configuration.Password
            }.ToString();
        }

        public override string GetInstanceConnectionString()
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = Configuration.Host,
                InitialCatalog = Configuration.InstanceName,
                IntegratedSecurity = false,
                UserID = Configuration.Username,
                Password = Configuration.Password
            }.ToString();
        }

        public override void OnInitialized()
        {
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = Configuration.Host,
                InitialCatalog = Configuration.InstanceName,
                IntegratedSecurity = false,
                UserID = Configuration.Username,
                Password = Configuration.Password
            };

            var entityBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ToString(),
                Metadata =
                    @"res://*/Database.EntityFramework.KonfDBModel.csdl|res://*/Database.EntityFramework.KonfDBModel.ssdl|res://*/Database.EntityFramework.KonfDBModel.msl"
            };

            _configurationStore = new ConfigurationDataStore(entityBuilder.ToString());
        }

        public override DatabaseCheckAction GetDatabaseCheckAction()
        {
            return new MsSqlCheckAction();
        }

        public override DatabaseCreateAction GetDatabaseCreateAction()
        {
            return new MsSqlCreateAction();
        }

        public override DatabaseObjectsCheckAction GetDatabaseObjectsCheckAction()
        {
            return new MsSqlObjectsCheckAction();
        }

        public override DatabaseSchemaCreateAction GetDatabaseSchemaCreateAction()
        {
            return new MsSqlSchemaCreateAction();
        }

        public override DatabaseTableCreateAction GetDatabaseTableCreateAction()
        {
            return new MsSqlTableCreateAction();
        }

        public override DatabaseReferenceDataAction GetDatabaseReferenceDataAction()
        {
            return new MsSqlReferenceDataAction();
        }

        public override DatabaseInitializedAction GetDatabaseInitializedAction()
        {
            return new MsSqlInitializedAction();
        }

        public override Dictionary<string, object> GetDataDictionary()
        {
            return new Dictionary<string, object>
            {
                {"$ResourceAssembly$", Assembly.GetAssembly(GetType())}
            };
        }
    }
}