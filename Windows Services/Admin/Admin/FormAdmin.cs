/*
*Filename:		FormAdmin.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormAdmin, this form is used to allow admin to: 
*               Edit questions and answers
*               See the current status of all live participants
*               View the leader board
*               view the question number, question text, average time to answer correctly, 
*               percentage of answering the question correctly, and 
*               a histogram to show the average length of time needed to answer each question correctly 
*               using excel
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
    /// NAME	:	FormAdmin
    /// PURPOSE :   This class is used to allow admin to :
    ///             Edit questions and answers
    ///             See the current status of all live participants
    ///             View the leader board
    ///             view the question number, question text, average time to answer correctly, 
    ///             percentage of answering the question correctly, 
    ///             and a histogram to show the average length of time needed to answer each question correctly
    public partial class FormAdmin : Form
    {
        private string myConnectionString;          // string used for connection
        private string ipAddress;                   // used to hold ip address
        private string userName;                    // used to hold user name
        private string passWord;                    // used to hold password
        private bool isConnect;                     // indicate whether it is connected or not
        private FormQAList QAList;                  // form used to show all questions
        private FormStatus userStatus;              // form used to show user status
        private FormLeaderboard leaderboard;        // form used to show leader board
        private ExcelApp excelFile;                 // used for create excel
        private DAL dal;                            // contains methods to talk to database


        ///Function:		FormAdmin constructor
        ///Description:     called to initiate an instance
        ///Parameters:      NONE
        ///Return Values:   NONE
        public FormAdmin()
        {
            InitializeComponent();
            isConnect = false;
            dal = new DAL();
        }



        ///Function:		btnConnect_Click
        ///Description:     this method is called when user click the connect button
        ///                 it will first check whether user entered all fields,
        ///                 if yes, it will try to connect
        ///                 if not, it will display an error message
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnConnect_Click(object sender, EventArgs e)
        {
            // IP address can't be empty
            if (txtBoxIP.Text == "")
            {
                MessageBox.Show("IP address cannot be empty");
                txtBoxIP.Focus();
            }
            else if (txtBoxName.Text == "")
            {
                MessageBox.Show("Name cannot be empty");        // user name can't be empty
                txtBoxName.Focus();
            }
            else if (txtBoxPassword.Text=="")
            {
                MessageBox.Show("Password cannot be empty");  // Password can't be empty
                txtBoxPassword.Focus();
            }
            else
            {
                ipAddress = this.txtBoxIP.Text;
                userName = txtBoxName.Text;
                passWord = txtBoxPassword.Text;
                // try to connect to the database
                string result = dal.ConnectToDB(ipAddress, userName, passWord);
                if (result == "")
                {
                    this.lblConnectInfo.Text = "Connected to the database"; // indicate connection success
                    this.btnConnect.Enabled = false;                // disable the connect button
                    this.txtBoxPassword.Enabled = false;            // disable the textboxes
                    this.txtBoxName.Enabled = false;
                    this.txtBoxIP.Enabled = false;
                    this.btnCheckStatus.Enabled = true;             // allow to check user status
                    this.btnEdit.Enabled = true;                    // allow to edit questions and answers
                    this.btnExcel.Enabled = true;                   // allow to view question answer length
                    this.btnLeaderboard.Enabled = true;             // allow to view the leaderboard
                }
                else
                {
                    this.lblConnectInfo.Text = result;
                }
            }
        }



        ///Function:		btnEdit_Click
        ///Description:     this method is called when user want to edit the questions and answers
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnEdit_Click(object sender, EventArgs e)
        {
            List<string> strList = dal.GetQuestionList();      // to get the list of all the questions

            // new the FormQAList, pass in the question list
            QAList = new FormQAList(strList, dal);
            // display the FormQAList
            QAList.ShowDialog();
        }
        


        ///Function:		btnLeaderboard_Click
        ///Description:     this method is called when user want to view the leaderboard
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            // new an instance of FormLeaderboard
            leaderboard = new FormLeaderboard(dal);
            leaderboard.ShowDialog();   // display the leaderboard
        }



        ///Function:		btnExcel_Click
        ///Description:     this method is called when user want to view the question report in excel
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnExcel_Click(object sender, EventArgs e)
        {
            string result;

            // new an instance of ExcelApp
            excelFile = new ExcelApp();
            result = excelFile.createExcel(dal);

            // if something happen, let user know
            if(result!="")
            {
                MessageBox.Show(result, "SET Test");
            }
        }



        ///Function:		btnExcel_Click
        ///Description:     this method is called when user want to view the question report in excel
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnCheckStatus_Click(object sender, EventArgs e)
        {
            userStatus = new FormStatus(dal);
            userStatus.ShowDialog();
        }
    }
}
