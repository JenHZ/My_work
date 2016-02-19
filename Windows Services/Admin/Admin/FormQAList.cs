/*
*Filename:		FormQAList.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormQAList, this form is used to show the admin
*               with the question text and question ID
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;




namespace Admin
{
    public partial class FormQAList : Form
    {
        /// NAME	:	FormQAList
        /// PURPOSE :   This class is used to show the admin with the question text and question ID
        private const int NUMOFQUESTION = 10;   // number of questions
        private List<RadioButton> rdBtnList;    // list of all the radio buttons that used for questions
        private List<Label> labelList;          // list of labels that used to display all the question text
        private FormQADetails qAndADetail;      // form used to show the question details
        private string question;                // used for question text
        private List<string> choices;           // list of all the choices
        private char answer;                    // used to record the right answer
        private List<string> choiceID;          // used to record all choices
        private string result;                  // used to record result from database access
        private DAL dal;                        // contains methods to talk to database


        ///Function:		FormQAList constructor
        ///Description:     This method is called to initiate an instance
        ///                 display the question list 
        ///Parameters:      List<string> QuestionList: list to hold all the questions
        ///                 MySqlConnection cn: mysql connection
        ///                 MySqlCommand cm: mysql command
        ///                 MySqlDataReader r: mysql reader
        ///Return Values:   NONE
        public FormQAList(List<string> QuestionList,DAL dataAccess)
        {
            InitializeComponent();
            rdBtnList = new List<RadioButton>();    // list of all the radio buttons that used for questions
            labelList = new List<Label>();          // list of labels that used to display all the question text
            choices = new List<string>();           // used to record all choices
            choiceID = new List<string>();
            dal = dataAccess;
            
            // list of all the questions text
            labelList.Add(this.lblQ1);              
            labelList.Add(this.lblQ2);              
            labelList.Add(this.lblQ3);
            labelList.Add(this.lblQ4);
            labelList.Add(this.lblQ5);
            labelList.Add(this.lblQ6);
            labelList.Add(this.lblQ7);
            labelList.Add(this.lblQ8);
            labelList.Add(this.lblQ9);
            labelList.Add(this.lblQ10);

            // list of the radio buttons for all the questions 
            rdBtnList.Add(this.rdBtnQ1);
            rdBtnList.Add(this.rdBtnQ2);
            rdBtnList.Add(this.rdBtnQ3);
            rdBtnList.Add(this.rdBtnQ4);
            rdBtnList.Add(this.rdBtnQ5);
            rdBtnList.Add(this.rdBtnQ6);
            rdBtnList.Add(this.rdBtnQ7);
            rdBtnList.Add(this.rdBtnQ8);
            rdBtnList.Add(this.rdBtnQ9);
            rdBtnList.Add(this.rdBtnQ10);

     
            // assign the question text to each label
            for (int i=0; i<labelList.Count; i++)
            {
                labelList[i].Text = (i + 1).ToString() + ". " + QuestionList[i];
            }
                
        }



        ///Function:		btnEdit_Click
        ///Description:     This method is called when admin want to edit the question
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // will first check whether admin had picked one question
            if (rdBtnQ1.Checked != true && rdBtnQ2.Checked != true && rdBtnQ3.Checked != true
                && rdBtnQ4.Checked != true && rdBtnQ5.Checked != true && rdBtnQ6.Checked != true
                && rdBtnQ7.Checked != true && rdBtnQ8.Checked != true && rdBtnQ9.Checked != true 
                && rdBtnQ10.Checked != true)
            {
                // if not pick one, will display an error message
                var result = MessageBox.Show("You need to select a question", "SET Test");
            }
            else
            {
                // loop to find which question admin choose
                for (int i = 0; i < rdBtnList.Count; i++)
                {
                    if(rdBtnList[i].Checked==true)
                    {
                        GetQADetails(i + 1);        // call to get the detail of the question
                    }
                }
            }
        }



        ///Function:		GetQADetails
        ///Description:     This method is called to get the question text, the choice text and the right 
        ///                 answer for a specific question
        ///Parameters:      int whichOne: indicate which question
        ///Return Values:   NONE
        public void GetQADetails(int whichOne)
        {
            string result = "";

            StringComparison comparison = StringComparison.InvariantCulture;
            question = dal.GetQuestionText(whichOne);
            
            if(question.StartsWith("Exception: ", comparison))
            {
                MessageBox.Show(question);    // in case something wrong happen
            }


            result = dal.GetChoiceText(choices, choiceID, whichOne);
            
            if(result !="")
            {
                MessageBox.Show(result);    // in case something wrong happen
            }


            int ret = dal.GetCorrectAnswer(whichOne);
            
            
            // transfer the choice id to a A, B, C, or D
            for (int i = 0; i < choiceID.Count; i++)
			{
			    if(choiceID[i].Equals(ret.ToString()))
                {
                    answer=(char)(i+65);    // transfer to the letter
                }
			}

            // open a new thread to display the form that allow admin to sedit the question
            Thread th = new Thread(() => ShowDetailForm(whichOne));
            th.Start();
            this.Close();       // close the form 
        }



        ///Function:		ShowDetailForm
        ///Description:     This method is called to open a form that used to allow admin to edit the question 
        ///                 and the choice text for a specific question that they just chooses
        ///Parameters:      int whichOne: indicate which question
        ///Return Values:   NONE
        public void ShowDetailForm(int whichOne)
        {
            // new an instance of FormQADetails and display it
            qAndADetail = new FormQADetails(whichOne, question, choices[0], choices[1], choices[2], choices[3], answer, choiceID, dal);
            qAndADetail.ShowDialog();
        }
    }
}
