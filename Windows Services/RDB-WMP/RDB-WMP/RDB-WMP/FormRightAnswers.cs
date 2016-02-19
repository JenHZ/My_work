/*
*Filename:		FormRightAnswer.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormRightAnswer, this form is used to show user the right answer
*               for each question and the answer that user picked
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
    /// NAME	:	FormRightAnswer
    /// PURPOSE :   This form is used to show user the right answer for each question 
    ///             and the answer that user picked
    public partial class FormRightAnswers : Form
    {
        private string input;
        private const int ROUNDS = 10;          // how many questions
        private const int NUMOFSTRING = 4;      // how many choices for each question



        ///Function:		FormRightAnswers constructor
        ///Description:     called to initiate an instance
        ///                 the long string that hold the information of the right answer and user choice 
        ///                 will be parsed and fill into the listbox
        ///Parameters:      string inp: the long string that hold the information of the right answer and user choice 
        ///Return Values:   NONE
        public FormRightAnswers(string inp)
        {
            InitializeComponent();

            input = inp;

            int counter = 0;                        // used to record how many strings read

            string[] listQA = input.Split('|');     // parse the passed in string

            
            // loop to read the question text, the right answer and user choice for each question
            for (int i = 1; i <= ROUNDS; i++)
            {
                listBoxQA.Items.Add(i.ToString() + ". " + listQA[counter]);     // question text
                counter++;
                listBoxQA.Items.Add("A. " + listQA[counter]);                   // choice A
                counter++;
                listBoxQA.Items.Add("B. " + listQA[counter]);                   // choice B
                counter++;
                listBoxQA.Items.Add("C. " + listQA[counter]);                   // choice C
                counter++;
                listBoxQA.Items.Add("D. " + listQA[counter]);                   // choice D
                counter++;
                listBoxQA.Items.Add(listQA[counter]);                           // right answer and user choice
                counter++;
                listBoxQA.Items.Add(" ");                                       
            }
        }
    }
}
