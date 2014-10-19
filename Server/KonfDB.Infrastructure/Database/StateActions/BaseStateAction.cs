#region License and Product Information

// 
//     This file 'BaseStateAction.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Configuration.Providers.Database;
using KonfDB.Infrastructure.Database.Providers;
using KonfDB.Infrastructure.Workflow;

namespace KonfDB.Infrastructure.Database.StateActions
{
    public abstract class BaseStateAction : IStateAction<DatabaseStates>
    {
        public DatabaseProviderElement Configuration { get; set; }
        public string MasterConnectionString { get; set; }
        public string InstanceConnectionString { get; set; }

        public DatabaseStates Execute(DatabaseStates currentState, Dictionary<string, object> dataDictionary,
            params object[] data)
        {
            MasterConnectionString = dataDictionary["$MasterConnectionString$"].ToString();
            InstanceConnectionString = dataDictionary["$InstanceConnectionString$"].ToString();
            Configuration = dataDictionary["$Configuration$"] as DatabaseProviderElement;

            return ExecuteState(currentState, dataDictionary, data);
        }

        public abstract DatabaseStates ExecuteState(DatabaseStates currentState,
            Dictionary<string, object> dataDictionary,
            params object[] data);
    }
}