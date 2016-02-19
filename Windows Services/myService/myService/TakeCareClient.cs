/*
*Filename:		TakeCareClient.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for TakeCareClient class 
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace myService
{
    /// NAME	:	TakeCareClient
    /// PURPOSE :   This class is used to handle the request from user and send back related messages.
    public class TakeCareClient
    {

        public ManualResetEvent allDone = new ManualResetEvent(false);  // Thread signal.
        DAL dal;                                                        // used to communicate with MySQL server database.
        public bool Done { get; set; }                                  // used to indicate that the service is shut down



        ///Function:		TakeCareClient -- constructor
        ///Description:     to instantiate a TakeCareClient object and initialize some attributes.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public TakeCareClient()
        {
            dal = new DAL();
            Done = false;
        }



        ///Function:		StartListening
        ///Description:     to start a socket and waiting for client to connect.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public void StartListening()
        {
            byte[] bytes = new Byte[1024];              // Data buffer for incoming data.

            // Establish the local endpoint for the socket.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);

            // Fire up a thread to check if the service is shut down.
            Thread checkThread = new Thread(new ThreadStart(checkDone));
            checkThread.Start();

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                // infinite loop to wait for new connections.
                while (!Done)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Logger.Log("Waiting for a connection...");                               // used for debug
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Logger.Log("Exception:" + e.ToString());
            }

            Logger.Log("\nProgram is stopped...");                                          // used for debug
        }



        ///Function:		checkDone
        ///Description:     a thread only used to check the state of done and signal the main thread.
        ///Parameters:      NONE
        ///Return Values:   NONE
        private void checkDone()
        {
            // check done every 1 second.
            while(!Done)
            {
                Thread.Sleep(1000);
                // signal the main thread to continue
                if(Done)
                {
                    allDone.Set();
                }
            }
        }



        ///Function:		AcceptCallback
        ///Description:     a thread to deal with a single user after the connection has been built
        ///Parameters:      IAsyncResult ar:    hold the information
        ///Return Values:   NONE
        public void AcceptCallback(IAsyncResult ar)
        {
            Logger.Log("Someone connected!");                                                   // used for debug
            
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object and initialize some data members.
            StateObject state = new StateObject();
            state.ScoreDone = new ManualResetEvent(false);
            state.NameDone = new ManualResetEvent(false);
            state.NameDone.Reset();
            state.WorkSocket = handler;
            
            // Create two Lists to hold all the question and choices from database
            List<Questions> questions = dal.GetAllQuestions();
            List<Choices> choices = dal.GetAllChoices();

            // create two lists to hold right choices and user's choices
            List<char> userChoices = new List<char>();
            List<char> rightChoices = new List<char>();

            // recieve user's name in a asynchronous method -- GetUserName
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(GetUserName), state);

            try
            {
                // for loop to send 10 questions to user
                for (int i = 0; i < 10; i++)
                {
                    state.ScoreDone.Reset();
                    String question = questions[i].QuestionText + "|";
                    for (int j = 0; j < 4; j++)
                    {
                        // append the text of choices to our string
                        String choice = choices[i * 4 + j].ChoiceText;
                        question += choice + "|";

                        // find the sequence of the right choice in the 4 chocies
                        if (choices[i * 4 + j].ChoiceID == questions[i].CorrectChoiceID)
                        {
                            rightChoices.Add((char)(j + 65));
                        }
                    } // end for loop

                    // send question and choices to the user
                    handler.Send(Encoding.UTF8.GetBytes(question));

                    // wait here until we get the right userID
                    state.NameDone.WaitOne();

                    // clear the buffer and assign current questionID and rightChoice to state object 
                    // so we can calculate them asynchronously 
                    Array.Clear(state.Buffer, 0, StateObject.BufferSize);
                    state.QuestionID = i + 1;
                    state.RightChoice = (int)(rightChoices[i] - 64);

                    // recieve user's chocie for this question in a asynchronous method and then wait until we get the score -- CalculateScore
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(CalculateScore), state);
                    state.ScoreDone.WaitOne();

                    // check if user is still there
                    if (state.Done == true)
                    {
                        // send a signal back to user if user wants to quit
                        handler.Send(Encoding.UTF8.GetBytes("c"));
                        break;
                    }

                    if (state.UserChoice > 0)
                    {
                        userChoices.Add((char)(state.UserChoice + 64));
                    }
                    else
                    {
                        userChoices.Add(' ');
                    }
                    Logger.Log("Add one user choice.");
                } // end for loop

                // check if user is still there
                if (state.Done == false)
                {
                    // send total score back to this user.
                    String userTotalScore = "Your total score is: " + dal.GetTotalScoreForSingleUser(state.UserID);
                    handler.Send(Encoding.UTF8.GetBytes(userTotalScore));
                    Logger.Log("Score sent");
                    String rightAnswer = "";

                    // send back the whole right answer and user choices list back 
                    for (int i = 0; i < 10; i++)
                    {
                        rightAnswer += questions[i].QuestionText + "|";
                        for (int j = 0; j < 4; j++)
                        {
                            rightAnswer += choices[i * 4 + j].ChoiceText + "|";
                        }

                        rightAnswer += "The right answer is: " + rightChoices[i] + ". ";
                        rightAnswer += "Your answer is: " + userChoices[i] + "|";
                    } // end for loop

                    handler.Send(Encoding.UTF8.GetBytes(rightAnswer));
                    Logger.Log("Answer sent");
                    int recv = handler.Receive(state.Buffer);
                    String client = Encoding.UTF8.GetString(state.Buffer, 0, recv);
                    Logger.Log("Recieved: " + client);

                    if(client == "l")
                    {
                        // get ranking from database and send it back to the user
                        String ranking = dal.GetLeaderBoard();
                        handler.Send(Encoding.UTF8.GetBytes(ranking));
                        Logger.Log("Ranking sent");
                    }
                } // end if
            } 
            // catch exceptions here
            catch(SocketException se)
            {
                Logger.Log("Socket Exception: " + se.Message);
            }
            catch(Exception e)
            {
                Logger.Log("Exception: " + e.Message);
                handler.Send(Encoding.UTF8.GetBytes("c"));
            }

            // user has left, we are going to update their status in the database
            dal.UpdateUser(state.UserID);
            Logger.Log("user left: " + state.UserID);
            if(handler.Connected)
            {
                handler.Close();
            }
        }



        ///Function:		CalculateScore
        ///Description:     a asynchronous method to calculate the score according to user's choice and the time spent.
        ///                 then save the result into database
        ///Parameters:      IAsyncResult ar:    hold the information
        ///Return Values:   NONE
        public void CalculateScore(IAsyncResult ar)
        {
            String[] content = new String[2];

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            // process if we get something
            if (bytesRead > 0)
            {
                // Store the data received.
                String strFromClient = Encoding.UTF8.GetString(state.Buffer);
                content = strFromClient.Split('|');
                Logger.Log(content[0] + content[1]);                                    // used for debug

                // parse numbers from client
                int userChoice = int.Parse(content[0]); 
                state.UserChoice = userChoice;
                int score = int.Parse(content[1]);
                
                // user wants to quit, we signal the main thread to continue and change the flag
                if(userChoice == 5)
                {
                    state.Done = true;
                    state.ScoreDone.Set();
                }

                // calculate score and save to database
                else
                {
                    // user got the right one
                    if (userChoice == state.RightChoice)
                    {
                        score = 20 - score;
                        Logger.Log("right, score is" + score);
                    }
                    else
                    {
                        score = 0;
                        Logger.Log("wrong");
                    } // end if

                    // create an UserChoice object to write to database
                    state.UserScore = score;
                    UserChoice uc = new UserChoice();
                    uc.QuestionID = state.QuestionID;
                    uc.UserID = state.UserID;
                    uc.Score = score;
                    Logger.Log("" + uc.QuestionID + "," + uc.UserID + "," + uc.Score);  // used for debug

                    // signal the main thread to continue
                    state.ScoreDone.Set();

                    // then we add it into database
                    dal.AddUserScore(uc);
                } // end if
            } // end if
        }



        ///Function:		GetUserName
        ///Description:     a asynchronous method to get the name of this client and get his ID.
        ///                 then save the result into database
        ///Parameters:      IAsyncResult ar:    hold the information
        ///Return Values:   NONE
        public void GetUserName(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            // process if we get something
            if (bytesRead > 0)
            {
                // Store the data received.
                state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                content = state.Sb.ToString();

                // create an Users object to write to database
                Users user = new Users();
                user.Status = 1;
                user.UserName = content;

                // we can only get the ID after inserting it into database
                user.UserID = dal.AddUser(user);
                state.UserID = user.UserID;

                Logger.Log("UserID: " + state.UserID);      // used for debug
                // signal the main thread to continue after we get the ID
                state.NameDone.Set();
            }
        }

    }

}
