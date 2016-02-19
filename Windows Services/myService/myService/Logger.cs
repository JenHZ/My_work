/*
*Filename:		Logger.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for Logger class 
*/



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	Logger
    /// PURPOSE :   This class is used to put information into our log file so we can know what happened.
    public static class Logger
    {
        ///Function:		Log
        ///Description:     to put some text information into log file
        ///Parameters:      string message:    some informative informaiton
        ///Return Values:   NONE
        public static void Log(string message)
        {
            // create a EventLog here
            EventLog serviceEventLog = new EventLog();
            if (!EventLog.SourceExists("MyEventSource"))
            {
                EventLog.CreateEventSource("MyEventSource", "MyEventLog");
            }

            // tring to log here
            serviceEventLog.Source = "MyEventSource";
            serviceEventLog.Log = "MyEventLog";
            serviceEventLog.WriteEntry(message);
        }
    }
}
