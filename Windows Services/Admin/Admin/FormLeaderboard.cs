/*
*Filename:		FormLeaderboard.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormLeaderboard, this form is used to show the admin
*               with the leaderboard at the real time
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;




namespace Admin
{
    /// NAME	:	FormLeaderboard
    /// PURPOSE :   This class is used to show the admin with the leaderboard at the real time
    public partial class FormLeaderboard : Form
    {
        private string input;                   // used to hold the result from the database
        private DataSet leaderboardDataSet;     // dataset for the result
        private DataTable leaderboardTable;     // table to display
        private DataColumn column1;             // first column of the table
        private DataColumn column2;             // second column of the table
        private DAL dal;                        // contains methods that will be used to access database


        ///Function:		FormLeaderboard constructor
        ///Description:     called to initiate an instance
        ///                 the dataset, datatable and columns will be initiated
        ///                 the title of the table will be set
        ///                 the long string that hold the information of the leardboard will be parsed and fill 
        ///                 into the table
        ///Parameters:      DAL DataAccessLayer: instance of DAL that will be used to access database
        ///Return Values:   NONE
        public FormLeaderboard(DAL DataAccessLayer)
        {
            InitializeComponent();

            dal = DataAccessLayer;
            
            // initiated the dataset, datatable and columns
            leaderboardDataSet = new DataSet();
            leaderboardTable = new DataTable();
            column1 = new DataColumn("Player Name");
            column2 = new DataColumn("Score");

            // add the table to the dataset and add the columns to the table
            leaderboardDataSet.Tables.Add(leaderboardTable);
            leaderboardTable.Columns.Add(column1);
            leaderboardTable.Columns.Add(column2);

            int counter = 0;

            // try to get the information of the leaderboard
            input = dal.GetLeaderBoard();
            // parse the string
            string[] leaderboard = input.Split('|');

            // fill the table using the information that just parsed
            for (int i = 0; i < leaderboard.Length / 2; i++)
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
