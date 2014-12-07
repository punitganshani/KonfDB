#region License and Product Information

// 
//     This file 'WithConfigurationValues.cs' is part of KonfDB application - 
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
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDBCF;

namespace KonfDB.RefSamples.ClientFramework
{
    public class WithConfigurationValues
    {
        public static void GetAppConfiguration()
        {
            try
            {
                var userToken = CConnectionFactory.GetUserToken();
                var commandService = CConnectionFactory.GetInstance();
                // If we got back a token, means user was authenticated
                if (userToken != null)
                {
                    var output = commandService.ExecuteCommand("get /app:6 /env:8 /region:8 /server:9", userToken);
                    if (output != null)
                    {
                        var parameters = (List<ConfigurationModel>) output.Data;
                        parameters.ForEach(param => Console.WriteLine(param.ParameterName + "=" + param.ParameterValue));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}