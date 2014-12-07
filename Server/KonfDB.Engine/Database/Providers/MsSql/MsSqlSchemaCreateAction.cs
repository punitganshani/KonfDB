#region License and Product Information

// 
//     This file 'MsSqlSchemaCreateAction.cs' is part of KonfDB application - 
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
using System.Reflection;
using System.Text;
using KonfDB.Infrastructure.Database.StateActions;
using KonfDB.Infrastructure.Extensions;
using KonfDB.Infrastructure.Shell;

namespace KonfDB.Engine.Database.Providers.MsSql
{
    public class MsSqlSchemaCreateAction : DatabaseSchemaCreateAction
    {
        public override void CreateSchema(string schemaName, Dictionary<string, object> dataDictionary, object[] data)
        {
            var assembly = dataDictionary["$ResourceAssembly$"] as Assembly;

            var builder = new StringBuilder();
            builder.AppendFormat("IF NOT EXISTS (select * from sys.schemas WHERE name ='{0}')", schemaName);
            builder.AppendLine("BEGIN");
            builder.AppendFormat("exec('{0}')",
                Configuration.Transform(assembly.GetEmbeddedResource("MsSql", String.Format("Schema.{0}", schemaName))));
            builder.AppendLine("END");

            using (var connection = new SqlConnection(InstanceConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = builder.ToString();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    HostContext.Current.Log.Error("Error while creating table", ex);
                }
            }
        }
    }
}