/*
*Filename:		UserChoice.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for UserChoice class 
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	UserChoice
    /// PURPOSE :   This class is used to represent a score got by user on a certain question.
    class UserChoice
    {
        private int userID;             // used to hold userID
        private int questionID;         // used to hold the questionID
        private int score;              // used to hold the score made by user



        /// accessor and mutator of userID.
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }



        /// accessor and mutator of questionID.
        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }



        /// accessor and mutator of score.
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
    }
}
