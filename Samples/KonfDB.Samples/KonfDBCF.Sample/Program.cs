using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonfDB.Infrastructure.Services;


namespace KonfDBCF.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create connection to Command Service of KonfDB based on settings
                // in app.config file
                ICommandService commandService = ConnectionFactory.GetInstance();

                // Get the user token if authenticated
                var userToken = ConnectionFactory.GetUserToken();

                // var commands = commandService.GetCommandsStartingWith("");

                // If we got back a token, means user was authenticated
                if (userToken != null)
                {
                    var output = commandService.ExecuteCommand("help", userToken);
                    Console.WriteLine(output.DisplayMessage);
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
