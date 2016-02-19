/*
*Filename:		Program.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The application is a service running on windows operating system which is waiting for connections from user.
*				This service connects to MySQL server database to provide questions and choices to user.
*				This service calculate the score of each question and stored it back to database
*				This service will provide the total score of user after each game.
*				This service will provide leaderboard of all users
*				This service will send back all correct answers for all questions to user.
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // instantiate a service and start it.
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new myService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
