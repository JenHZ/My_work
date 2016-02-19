/*
*Filename:		FormLeaderboard.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormLeaderboard, this form is used to show user the leaderboard
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RDB_WMP
{
    /// NAME	:	FormLeaderboard
    /// PURPOSE :   This form is used to show user the leaderboard
    public partial class FormLeaderboard : Form
    {
        private string input;                   // used to hold the whole string of leaderboard infomation
        private DataSet leaderboardDataSet;     // dataset used to display the leaderboard
        private DataTable leaderboardTable;     // table used to display the leaderboard
        private DataColumn column1;             // the first column of the table
        private DataColumn column2;             // the second column of the table



        ///Function:		FormLeaderboard constructor
        ///Description:     called to initiate an instance
        ///                 the dataset, datatable and coloums will be initiated
        ///                 the title of the table will be set
        ///                 the long string that hold the information of the leardboard will be parsed and fill 
        ///                 into the table
        ///Parameters:      string inp: the long string that hold the information of the leardboard
        ///Return Values:   NONE
        public FormLeaderboard(string inp)
        {
            InitializeComponent();

            input = inp;

            // initiated the dataset, datatable and coloums
            leaderboardDataSet = new DataSet();
            leaderboardTable = new DataTable();
            column1 = new DataColumn("Plyaer Name");
            column2 = new DataColumn("Score");
            // add the tabe to the dataset and add the columns to the table
            leaderboardDataSet.Tables.Add(leaderboardTable);
            leaderboardTable.Columns.Add(column1);
            leaderboardTable.Columns.Add(column2);

            int counter = 0;                    // used to record how many strings read

            // parse the passed in string
            string[] leaderboard = input.Split('|');

            // fill the table using the information that just parsed
            for (int i = 0; i < leaderboard.Length/2; i++)
            {
                DataRow dr = leaderboardTable.NewRow();
                dr[0] = leaderboard[counter];
                counter++;
                dr[1] = leaderboard[counter];
                counter++;
                leaderboardTable.Rows.Add(dr);
            }

            // bind the source
            this.dataGridViewLeaderboard.DataSource = leaderboardTable;
        }
    }
}
