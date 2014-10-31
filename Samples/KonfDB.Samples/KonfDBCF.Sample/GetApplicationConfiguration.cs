using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDB.Infrastructure.Services;

namespace KonfDBCF.Sample
{
    public class GetApplicationConfiguration
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
                        var parameters = (List<ConfigurationModel>)output.Data;
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
