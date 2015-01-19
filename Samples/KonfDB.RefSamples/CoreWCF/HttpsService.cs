#region License and Product Information

// 
//     This file 'HttpsService.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDBCF;
using KonfDBCF.Configuration;

namespace KonfDB.RefSamples.CoreWCF
{
    public class HttpsService
    {
        public static void GetAppConfiguration()
        {
            ClientConfig config = new ClientConfig();
            config.Runtime.User.Username = "konfdbuser_ro";
            config.Runtime.User.Password = "konfdbuser_ro";
            config.Runtime.Client.Host = "localhost";
            config.Runtime.Client.Port = 8880;
            config.Runtime.Client.Type = EndPointType.HTTP;

            var commandService = ConnectionFactory.GetInstance(config); 
            var output = commandService.Value.ExecuteCommand("get /app:6 /env:8 /region:8 /server:9");
            if (output != null)
            {
                var parameters = (List<ConfigurationModel>) output.Data;
                parameters.ForEach(param => Console.WriteLine(param.ParameterName + "=" + param.ParameterValue));
            }

            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}