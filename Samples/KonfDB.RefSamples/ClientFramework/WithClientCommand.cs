using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KonfDB.Infrastructure.Database.Entities.Configuration;
using KonfDBCF;
using KonfDBCF.Commands;

namespace KonfDB.RefSamples.ClientFramework
{
    public class WithClientCommand
    {
        public static void GetAppConfiguration()
        {
            try
            {

                var commandService = ConnectionFactory.GetInstance(new FileInfo("konfdbc.json"));
                // If we got back a token, means user was authenticated
                if (commandService != null)
                {
                    var output = commandService.Value.ExecuteCommand(new Get { Application = "6", Environment = "8", Region = "8", Server = "9" });
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
