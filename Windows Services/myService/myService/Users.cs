/*
*Filename:		Users.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for Users class 
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	Users
    /// PURPOSE :   This class is used to represent a user who is playing this game.
    class Users
    {
        private int userID;             // used to hold userID
        private String userName;        // used to hold user name
        private int status;             // used to hold the status of this user



        /// accessor and mutator of userID.
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }



        /// accessor and mutator of userName.
        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }



        /// accessor and mutator of status.
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}
