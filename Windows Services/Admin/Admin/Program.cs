/*
*Filename:		Program.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The application is a admin program, this program allow admin to:
*               Edit questions and answers
*               See the current status of all live participants
*               View the leaderboard
*               view the question number, question text, average time to answer correctly, 
*               percentage of answering the question correctly, and 
*               a histogram to show the average length of time needed to answer each question correctly 
*               using excel
*/



using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Admin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormAdmin());
        }
    }
}
