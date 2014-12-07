#region License and Product Information

// 
//     This file 'MsSqlObjectsCheckAction.cs' is part of KonfDB application - 
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
using System.Data.SqlClient;
using KonfDB.Infrastructure.Database.StateActions;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Database.Providers.MsSql
{
    public class MsSqlObjectsCheckAction : DatabaseObjectsCheckAction
    {
        public override bool SchemasExist(string[] schemas, Dictionary<string, object> dataDictionary,
            params object[] data)
        {
            using (var connection = new SqlConnection(InstanceConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = String.Format("select count(1) from sys.schemas WHERE name IN ('{0}')",
                    String.Join("','", schemas));
                try
                {
                    var schemaCount = (int) command.ExecuteScalar();
                    return (schemaCount == schemas.Length);
                }
                catch (Exception ex)
                {
                    HostContext.Current.Log.Error("Error while validating schemas", ex);
                }
            }

            return false;
        }

        public override bool TablesExist(string schema, string[] tables, Dictionary<string, object> dataDictionary,
            params object[] data)
        {
            using (var connection = new SqlConnection(InstanceConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                    String.Format("SELECT count(1) FROM sys.tables WHERE name IN  ('{0}') AND type = 'U' ",
                        String.Join("','", tables));
                try
                {
                    var tableCount = (int) command.ExecuteScalar();
                    return (tableCount == tables.Length);
                }
                catch (Exception ex)
                {
                    HostContext.Current.Log.Error("Error while validating tables", ex);
                }
            }

            return false;
        }

        public override bool ReferencesExist(Dictionary<string, object> dataDictionary, params object[] data)
        {
            return false;
        }
    }
}