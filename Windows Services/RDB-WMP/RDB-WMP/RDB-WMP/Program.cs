/*
*Filename:		Program.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The application is a client program. 
*               User is presented with a set of questions (one at a time) and multiple choice answers
*               Zero points are awarded for the wrong answer
*               Points are awarded for the correct answer based on how fast the answer was provided
*               At the end of the questions, a score is provided with a leaderboard.
*               Correct answers are only provided when the game ends.
*/




using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RDB_WMP
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
            Application.Run(new FormPlayer());
        }
    }
}
