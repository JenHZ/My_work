/*
*Filename:		FormQADetails.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormQADetails, this form is used to show the admin
*               with the question text and the choice text for the question they chose
*               and will allow admin to edit the question, the choice and the right choice
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
    /// NAME	:	FormQADetails
    /// PURPOSE :   This class is used to show the admin with the question text and the choice text for the question
    public partial class FormQADetails : Form
    {
        private string question;            // used to hold the question text
        private List<string> choices;       // used to hold the all choices
        private string questionID;          // used to record the question ID
        private char answer;                // used to record the right answer
        private List<string> choiceID;
        private List<TextBox> txtBoxList;   // used to record all textboxes
        private DAL dal;                    // contains methods to talk to database


        ///Function:		FormQADetails constructor
        ///Description:     called to initiate an instance
        ///                 The question text and choice text will be displayed in the textboxes
        ///Parameters:      int qID: the question ID
        ///                 string que: the question text
        ///                 string c1: choice A text
        ///                 string c2: choice B text
        ///                 string c3: choice C text
        ///                 string c4: choice D text
        ///                 char ans: the right answer
        ///                 List<string> cID: list of the choice id 
        ///                 DAL dataAccess: data access class
        ///Return Values:   NONE
        public FormQADetails(int qID, string que, string c1, string c2, string c3, string c4, char ans, List<string> cID, DAL dataAccess)
        {
            InitializeComponent();
            questionID = qID.ToString();        // assign the question id
            question = que;                     // assign the question text
            choices = new List<string>();       
            choices.Add(c1);                    // add the choice to the choice list
            choices.Add(c2);
            choices.Add(c3);
            choices.Add(c4);

            answer = ans;                       // assign the right answer
            choiceID = cID;                     // assign the choice id
            dal = dataAccess;

            // add the textbox for choice text to the list
            txtBoxList = new List<TextBox>();
            txtBoxList.Add(this.txtBoxChoice1);     
            txtBoxList.Add(this.txtBoxChoice2);
            txtBoxList.Add(this.txtBoxChoice3);
            txtBoxList.Add(this.txtBoxChoice4);

            this.lblQuestionID.Text = "Question " + qID;
            // assign the value of the question text to te text box
            this.txtBoxQuestion.Text = question;

            // assign the value of the choice text to te text box
            for (int i = 0; i < txtBoxList.Count; i++)
            {
                txtBoxList[i].Text = choices[i];
            }

            // assign the value of the right answer to te text box
            this.txtBoxAnswer.Text = answer.ToString();
        }



        ///Function:		btnSet_Click
        ///Description:     This method is called when admin want to save the change to the question or choice
        ///                 This method will first check the texkboc to make sure the value is not empty
        ///                 This method will also check the range of the right answer, as it can only be A, B, C or D
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnSet_Click(object sender, EventArgs e)
        {
            int dif = 0;            // used to check the difference between the new right answer and the previous
            bool status = true;     // used to record the status of setting
            string result = "";     // used to record error message

            // if user change the right answer
            if(!this.txtBoxAnswer.Text.ToString().ToUpper().Equals(answer.ToString()))
            {
                // check if admin enter a right answer other than A, B, C, or D
                if(!this.txtBoxAnswer.Text.ToString().ToUpper().Equals("A") &&
                    !this.txtBoxAnswer.Text.ToString().ToUpper().Equals("B") &&
                    !this.txtBoxAnswer.Text.ToString().ToUpper().Equals("C") &&
                    !this.txtBoxAnswer.Text.ToString().ToUpper().Equals("D"))
                {
                    // show an error message
                    MessageBox.Show("Answer can only be A, B, C, or D.", "SET Test");
                }
                else
                {
                    // calculate the difference between the new right answer and the previous one
                    if(this.txtBoxAnswer.Text.ToString().ToUpper().Equals("A"))
                    {
                        dif = 65 - answer;
                    }
                    else if(this.txtBoxAnswer.Text.ToString().ToUpper().Equals("B"))
                    {
                        dif = 66 - answer;
                    }
                    else if (this.txtBoxAnswer.Text.ToString().ToUpper().Equals("C"))
                    {
                        dif = 67 - answer;
                    }
                    else if (this.txtBoxAnswer.Text.ToString().ToUpper().Equals("D"))
                    {
                        dif = 68 - answer;
                    }


                    result = dal.UpdateRightAnswer(dif, questionID);
                    if(result!="")
                    {
                        MessageBox.Show(result); // in case something wrong happen
                        status = false;
                    }
                }
            }


            // if admin change the question text
            if(!this.txtBoxQuestion.Text.ToString().Equals(question))
            {
                // first need to check whether the textbox is empty
                if(this.txtBoxQuestion.Text=="")
                {
                    // display an error message
                    MessageBox.Show("Question cannot be empty", "SET Test");
                }
                else
                {
                    result = dal.UpdateQuestion(txtBoxQuestion.Text, questionID);
                    if(result!="")
                    {
                        MessageBox.Show(result);        // in case something wrong happen
                        status = false;
                    }
                }
            }

            // loop to check each textbox for the choice text
            for (int i = 0; i < txtBoxList.Count; i++)
            {
                // check if admin change the choice text 
                if (!txtBoxList[i].Text.ToString().Equals(choices[i]))
                {
                    // first need to check whether the textbox is empty
                    if (this.txtBoxList[i].Text == "")
                    {
                        // display an error message
                        MessageBox.Show("Choice cannot be empty", "SET Test");
                    }
                    else
                    {
                        result = dal.UpdateChoice(txtBoxList[i].Text, choiceID[i]);
                        if (result != "")
                        {
                            MessageBox.Show(result); // in case something wrong happen
                            status = false;
                        }
                    }
                }
            }

            // if all editing is success, will display a message
            if(status)
            {
                MessageBox.Show("Set successfully", "SET Test");
            }

            this.Close();       // close the form
        }
    }
}
