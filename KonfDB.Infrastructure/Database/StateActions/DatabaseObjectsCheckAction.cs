#region License and Product Information

// 
//     This file 'DatabaseObjectsCheckAction.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Database.Providers;

namespace KonfDB.Infrastructure.Database.StateActions
{
    public abstract class DatabaseObjectsCheckAction : BaseStateAction
    {
        public override DatabaseStates ExecuteState(DatabaseStates currentState,
            Dictionary<string, object> dataDictionary,
            params object[] data)
        {
            if (currentState == DatabaseStates.DatabaseCreated)
            {
                if (!SchemasExist(Metadata.Schemas, dataDictionary, data))
                    return DatabaseStates.SchemaMissing;
                return DatabaseStates.SchemaCreated;
            }
            if (currentState == DatabaseStates.SchemaCreated)
            {
                bool tablesExist = true;
                foreach (var schemaName in Metadata.Schemas)
                {
                    tablesExist &= TablesExist(schemaName, Metadata.SchemaMap[schemaName], dataDictionary, data);
                }

                if (!tablesExist)
                    return DatabaseStates.TablesMissing;
                return DatabaseStates.TablesCreated;
            }
            if (currentState == DatabaseStates.TablesCreated)
            {
                if (!ReferencesExist(dataDictionary, data))
                    return DatabaseStates.ReferencesMissing;
                return DatabaseStates.ReferenceDataPopulated;
            }

            // Should not reach here
            return DatabaseStates.UnknownState;
        }

        public abstract bool SchemasExist(string[] schemas, Dictionary<string, object> dataDictionary,
            params object[] data);

        public abstract bool TablesExist(string schema, string[] tables, Dictionary<string, object> dataDictionary,
            params object[] data);

        public abstract bool ReferencesExist(Dictionary<string, object> dataDictionary,
            params object[] data);
    }
}