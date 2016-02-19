/*
*Filename:		StateObject.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for StateObject class
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myService
{
    /// NAME	:	StateObject
    /// PURPOSE :   This class is used for reading client data asynchronously
    public class StateObject
    {
        public const int BufferSize = 1024;                 // Size of receive buffer.
        private byte[] buffer = new byte[BufferSize];       // Receive buffer from client.
        private Socket workSocket = null;                   // Client  socket.
        private StringBuilder sb = new StringBuilder();     // Received data string.

        private ManualResetEvent scoreDone = null;          // used to control asynchronous threads.
        private ManualResetEvent nameDone = null;           // used to control asynchronous threads.

        private int rightChoice;                            // used to hold the right choice of current question (1 to 4).
        private int userScore;                              // used to hold the score of current question (0 to 20)
        private int userChoice;                             // used to hold the choice made by user of current question (0 to 20)
        private int userID;                                 // used to hold the id of current user
        private int questionID;                             // used to hold the id of current question
        private bool done = false;                          // used to check if the user is still there



        /// accessor and mutator of workSocket.
        public Socket WorkSocket
        {
            get { return workSocket; }
            set { workSocket = value; }
        }



        /// accessor and mutator of buffer.
        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }



        /// accessor and mutator of sb.
        public StringBuilder Sb
        {
            get { return sb; }
            set { sb = value; }
        }



        /// accessor and mutator of rightChoice.
        public int RightChoice
        {
            get { return rightChoice; }
            set { rightChoice = value; }
        }



        /// accessor and mutator of userScore.
        public int UserScore
        {
            get { return userScore; }
            set { userScore = value; }
        }



        /// accessor and mutator of userChoice.
        public int UserChoice
        {
            get { return userChoice; }
            set { userChoice = value; }
        }



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



        /// accessor and mutator of done.
        public bool Done
        {
            get { return done; }
            set { done = value; }
        }



        /// accessor and mutator of scoreDone.
        public ManualResetEvent ScoreDone
        {
            get { return scoreDone; }
            set { scoreDone = value; }
        }



        /// accessor and mutator of nameDone.
        public ManualResetEvent NameDone
        {
            get { return nameDone; }
            set { nameDone = value; }
        }
    }
}
