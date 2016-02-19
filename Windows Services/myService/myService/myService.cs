/*
*Filename:		myService.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for myService class 
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace myService
{
    /// NAME	:	myService
    /// PURPOSE :   This class is the main part of service
    public partial class myService : ServiceBase
    {
        public Thread t;                // a thread running in the background of this service
        public TakeCareClient tcc;      // a TakeCareClient object which contains all functions of this service



        ///Function:		myService -- constructor
        ///Description:     to instantiate a myService object and initialize some attributes.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public myService()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
            tcc = new TakeCareClient();
            t = new Thread(new ThreadStart(tcc.StartListening));
        }



        ///Function:		OnStart
        ///Description:     is called when the the user starts this service
        ///Parameters:      string[] args:   
        ///Return Values:   NONE
        protected override void OnStart(string[] args)
        {
            // start the thread and log
            t.Start();
            Logger.Log("Service started.");
        }



        ///Function:		OnStop 
        ///Description:     is called when the user wants to stop this service
        ///Parameters:      string[] args:   
        ///Return Values:   NONE
        protected override void OnStop()
        {
            // signal the background thread to stop and wait it to finish
            tcc.Done = true;
            t.Join();
            Logger.Log("Service stopped.");
        }
    }
}
