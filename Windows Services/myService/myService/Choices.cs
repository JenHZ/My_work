/*
*Filename:		Choices.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for Choices class 
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	Choices
    /// PURPOSE :   This class is used to represent a choice for the user to choose
    class Choices
    {
        private int choiceID;           // used to hold choiceID
        private String choiceText;      // used to hold the text of a choice
        private int questionID;         // used to hold questionID associated with this choice


        /// accessor and mutator of choiceID.
        public int ChoiceID
        {
            get { return choiceID; }
            set { choiceID = value; }
        }



        /// accessor and mutator of choiceText.
        public String ChoiceText
        {
            get { return choiceText; }
            set { choiceText = value; }
        }



        /// accessor and mutator of questionID.
        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }
    }
}
