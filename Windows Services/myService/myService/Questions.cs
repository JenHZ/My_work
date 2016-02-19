/*
*Filename:		Questions.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for Questions class 
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	Questions
    /// PURPOSE :   This class is used to represent a question provided to the user.
    class Questions
    {
        private int questionID;                 // used to hold questionID
        private String questionText;            // used to hold the text of this question
        private int correctChoiceID;            // used to hold the id of correct choice for this quesiton



        /// accessor and mutator of questionID.
        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }



        /// accessor and mutator of questionText.
        public String QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }



        /// accessor and mutator of correctChoiceID.
        public int CorrectChoiceID
        {
            get { return correctChoiceID; }
            set { correctChoiceID = value; }
        }
    }
}
