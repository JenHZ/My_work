/*
*Filename:		FormStatus.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormStatus, this form is used to show the admin
*               with the alive user that is playing the game right now with the score 
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
    /// NAME	:	FormStatus
    /// PURPOSE :   This class is used to show the admin with the alive users and their score
    public partial class FormStatus : Form
    {
        private string input;                   // used to hold the result that read from the database
        private DataSet statusDataSet;          // dataset for the result 
        private DataTable statusTable;          // table to display
        private DataColumn column1;             // first column of the table
        private DataColumn column2;             // second column of the table
        private DAL dal;                        // contains methods that will be used to access database


        ///Function:		FormStatus constructor
        ///Description:     called to initiate an instance
        ///                 the dataset, datatable and columns will be initiated
        ///                 the title of the table will be set
        ///                 the long string that hold the information of the user status will be parsed and fill 
        ///                 into the table
        ///Parameters:      MySqlConnection cn: mysql connection
        ///                 MySqlCommand cm: mysql command
        ///                 MySqlDataReader r: reader
        ///Return Values:   NONE
        public FormStatus(DAL dataAccess)
        {
            InitializeComponent();
            StringComparison comparison = StringComparison.InvariantCulture;

            dal = dataAccess;

            // initiated the dataset, datatable and columns
            statusDataSet = new DataSet();
            statusTable = new DataTable();
            column1 = new DataColumn("Player Name");
            column2 = new DataColumn("Score");

            // add the table to the dataset and add the columns to the table
            statusDataSet.Tables.Add(statusTable);
            statusTable.Columns.Add(column1);
            statusTable.Columns.Add(column2);

            // try to get the information of the user status
            input = dal.GetUserStatus();

            if (input.StartsWith("Exception: ", comparison))
            {
                MessageBox.Show(input);    // in case something wrong happen
            }

            int counter = 0;

            // parse the string
            string[] leaderboard = input.Split('|');

            // fill the table using the information that just parsed
            for (int i = 0; i < leaderboard.Length / 2; i++)
            {
                DataRow dr = statusTable.NewRow();
                dr[0] = leaderboard[counter];
                counter++;
                dr[1] = leaderboard[counter];
                counter++;
                statusTable.Rows.Add(dr);
            }

            // bind the source
            this.dataGridViewStatus.DataSource = statusTable;
        }
        
    }
}
