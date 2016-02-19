/*
*Filename:		FormPlayer.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class FormPlayer, this form is used to: 
*               show user the question and choices, one question at a time.
*               After user finish all questions, this program allows user to see the right answer and leaderboard
*/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;



namespace RDB_WMP
{
    /// NAME	:	FormPlayer
    /// PURPOSE :   This form is used to show user the question and choices, one question at a time.
    ///             and after user finish all questions, this program allows user to see the right answer 
    ///             and leaderboard
    public partial class FormPlayer : Form
    {
        private String myName;                          // used to hold user name
        private IPAddress ipAddress;                    // used to hold IP address

        private const int BUFFERSIZE = 8192;
        private const int PORT = 5000;                  // port used for socket
        private const int ROUNDS = 10;                  // how many questions
        private const int NUMOFCHOICE = 4;              // how many choice for each quesiton

        private Socket server = null;                   // used to communicate with server
        private byte[] SentBytes;                       // used to hold information that will be send to the server
        private byte[] data = new byte[BUFFERSIZE];     // used to hold the information that will ge got from the server

        private TimeSpan ts;                            // used to deduct time as time passed
        private int counter;                            // used to record how many questions have been read
        private int timeElapsed;                        // used to record the passed time
        private bool AnswerSent;                        // indicate whether the answer has been sent to the server
        private string score;                           // used to hold user score that sent from the server
        private bool done;                              // used to indicate whether all questions have been answered
        private Thread th;
        private string question;                        // used to hold the text of the question
        private string choice1;                         // used to hold the text of choice 1
        private string choice2;                         // used to hold the text of choice 2
        private string choice3;                         // used to hold the text of choice 3
        private string choice4;                         // used to hold the text of choice 4
        delegate void MyCallback();                     // Delegate declaration for use in Invoke
        private FormRightAnswers formQA;                // the form used to show right answer
        private FormLeaderboard formLeaderboard;        // the form used to show leaderboard



        ///Function:		FormPlayer constructor
        ///Description:     called to initiate an instance
        ///Parameters:      NONE
        ///Return Values:   NONE
        public FormPlayer()
        {
            InitializeComponent();
            counter = 0;
            formQA = null;
            timeElapsed = 0;
            AnswerSent = false;
            done = false;
            ts = new TimeSpan(0, 0, 20);        // FOR DEBUG
        }



        ///Function:		FormPlayer_Load
        ///Description:     called the form is loading, it will first open a form to ask to user name and ip address
        ///                 after user enter the name and ip address, the form will show a greeting message 
        ///                 and send the user name to the server, and open a new thread to receive info from the server
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void FormPlayer_Load(object sender, EventArgs e)
        {
            // get user name and IP address
            FormAskName newname = new FormAskName();
            newname.ShowDialog();
            myName = newname.MyName;

            // name can not be null
            if(myName==null)
            {
                this.Close();
            }
            else
            {
                ipAddress = newname.IpAddress;
                server = newname.Server;
                ts = new TimeSpan(0, 0, 20);

                // make sure the name is not null
                if (server == null)
                {
                    this.Close();             // if not connect to the server, close the form
                }

                this.lblGreeting.Text = "Welcome, " + myName + "!";

                SentBytes = Encoding.UTF8.GetBytes(myName);
                server.Send(SentBytes);                         // send information to the server
                Array.Clear(SentBytes, 0, SentBytes.Length);
                QuestionTimer.Enabled = true;                   // enable the timer

                th = new Thread(() => getQuestion());           // new a thread to receive info from the server
                th.Start();
            }
        }



        ///Function:		getQuestion
        ///Description:     this method is used to receive information from the server
        ///                 it will first receive the question text and choices for each quesiton
        ///                 and then receive user total score, right answer and leaderboard
        ///Parameters:      NONE
        ///Return Values:   NONE
        private void getQuestion()
        {
            int recv = 0;
            string str ="";     // used to hold the string that get from the server
            string str2 = "";   // used to hold the string that get from the server


            // loop to receive the question text and choices for each quesiton
            while(counter < ROUNDS && !done)
            {
                // receive the information send from the server
                recv = server.Receive(data);
                
                // need to make sure user does not close the form
                if (Encoding.UTF8.GetString(data, 0, recv) != "c")
                {
                    // pasrse the string that sent from the server
                    String[] content = Encoding.UTF8.GetString(data, 0, recv).Split('|');


                    // assign the question text and choice text
                    question = content[0];
                    choice1 = content[1];
                    choice2 = content[2];
                    choice3 = content[3];
                    choice4 = content[4];

                    counter++;
                    ShowQuestions();        // show the question to the user
                }                
            }

            // receive the user total score, right answer and leaderboard
            if(!done)
            {
                // receive the information send from the server for score
                recv = server.Receive(data);
                score = Encoding.UTF8.GetString(data, 0, recv);

                // need to make sure user does not close the form
                if(score !="c")
                {
                    // receive the information of the right answer send from the server
                    try
                    {
                        // loop to read all the info about the right answer, and user choice
                        while (true)
                        {
                            recv = server.Receive(data, data.Length, 0);
                            str2 = Encoding.UTF8.GetString(data, 0, recv);
                            str = str + str2;           // append the string
                            if (recv <= data.Length)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    // new the FormRightAnswers instance
                    formQA = new FormRightAnswers(str);

                    // tell the server it want the leaderboard now
                    SentBytes = Encoding.UTF8.GetBytes("l");
                    Debug.WriteLine(SentBytes);
                    server.Send(SentBytes);   


                    // need to make sure user does not close the form
                    if (str != "c")
                    {
                        // receive the information of leadrboard send from the server
                        recv = server.Receive(data);
                        str = Encoding.UTF8.GetString(data, 0, recv);

                        //Debug.WriteLine(str);


                        formLeaderboard = new FormLeaderboard(str);
                        counter++;
                        ShowQuestions();        // show user the total score
                    }
                }
            }
        }



        ///Function:		ShowQuestions
        ///Description:     this method is used to show user the question text and choice when user answering the
        ///                 questions, and the total score after user finish all questions
        ///Parameters:      NONE
        ///Return Values:   NONE
        public void ShowQuestions()
        {
            // if user is still answering the quesitons
            if(counter <= ROUNDS)
            {
                // cross thread to change the UI of the form
                if (this.lblQuestion.InvokeRequired || this.rdBtnChoice1.InvokeRequired || this.rdBtnChoice2.InvokeRequired ||
                this.rdBtnChoice3.InvokeRequired || this.rdBtnChoice4.InvokeRequired)                      // InvokeRequired property is true if child thread
                {
                    MyCallback callback = new MyCallback(ShowQuestions);        // Callback is instance of delegate
                    Invoke(callback);
                }
                else
                {
                    // display the quesiton and answer
                    this.lblQuestion.Text = counter.ToString() + ". " + question;
                    this.rdBtnChoice1.Text = "A. " + choice1;
                    this.rdBtnChoice2.Text = "B. " + choice2;
                    this.rdBtnChoice3.Text = "C. " + choice3;
                    this.rdBtnChoice4.Text = "D. " + choice4;
                    this.lblTimeLeft.Text = "Time left: 0:0:20";

                    //Debug.WriteLine("get " + question + choice1 + choice2 + choice3 + choice4);
                }

                // begin to count the passed time
                ts = new TimeSpan(0, 0, 20);
                QuestionTimer.Enabled = true;
                lblTimeLeft.ForeColor = Color.Black;
                
                // uncheck all radio buttons
                rdBtnChoice1.Checked = false;
                rdBtnChoice2.Checked = false;
                rdBtnChoice3.Checked = false;
                rdBtnChoice4.Checked = false;
                timeElapsed = 0;
                AnswerSent = false;
            }
            else
            {
                // show the total score
                if(btnSubmit.InvokeRequired || groupBoxQuestion.InvokeRequired || lblResult.InvokeRequired ||
                    btnRightAnswer.InvokeRequired || btnLeaderboard.InvokeRequired)
                {
                    MyCallback callback = new MyCallback(ShowQuestions);        // Callback is instance of delegate
                    Invoke(callback);
                }
                else
                {
                    // disable the submit button
                    this.btnSubmit.Visible = false;
                    this.groupBoxQuestion.Visible = false;      // hid the question area
                    lblTimeLeft.Visible = false;                // hid the timer

                    this.lblResult.Visible = true;              // show the result
                    this.lblResult.Text = score;                

                    this.btnRightAnswer.Visible = true;         // enable the view right answer button
                    this.btnLeaderboard.Visible = true;         // enable the view leaderboard answer button
                }
            }
        }



        ///Function:		QuestionTimer_Tick
        ///Description:     this method is used to show user the timer for each question
        ///                 the time will change every second, and will ture red when only 5 second left
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void QuestionTimer_Tick(object sender, EventArgs e)
        {
            // string used to show the info of the timer
            String str = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
 
            lblTimeLeft.Text = "Time left: " + str;
 
            ts = ts.Subtract(new TimeSpan(0, 0, 1));        // deduct every second
            timeElapsed++;

            // ture red when only 5 second left
            if(ts.Seconds<5)
            {
                lblTimeLeft.ForeColor = Color.Red;
            }
            // when time is up, stop the timer and send server the user choice if not sent yet
            if(ts.Seconds<0)
            {
                QuestionTimer.Enabled = false;

                if(AnswerSent==false)
                {
                    // inidicate the user did not make any choice until time is up
                    SentBytes = Encoding.UTF8.GetBytes("0|0");
                    Debug.WriteLine(SentBytes);
                    server.Send(SentBytes);                         // send information to the server
                    AnswerSent = true;
                }
            }
        }



        ///Function:		btnSubmit_Click
        ///Description:     this method is called when user click the submit button
        ///                 it will first check whethe user pick a choice
        ///                 if yes, it will send the answer to the server
        ///                 if not, it will display an error message to the user
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // first check whethe user pick a choice
            if(rdBtnChoice1.Checked!=true && rdBtnChoice2.Checked!=true && rdBtnChoice3.Checked!=true 
                && rdBtnChoice4.Checked!=true)
            {
                // display an error message to the user
                var result = MessageBox.Show("Are you sure you want to submit without pick an option?", "SET Test", 
                    MessageBoxButtons.YesNo);

                // user want to submit without picking an answer
                if(result==DialogResult.Yes)
                {
                    SentBytes = Encoding.UTF8.GetBytes("0|0");      // tell server user did not choose
                    server.Send(SentBytes);                         // send information to the server
                    QuestionTimer.Enabled = false;
                    AnswerSent = true;
                }
            }
            else
            {
                // tell the server which choic user picked
                if (rdBtnChoice1.Checked == true)
                {
                    SentBytes = Encoding.UTF8.GetBytes("1|"+timeElapsed.ToString());
                    server.Send(SentBytes);                         // send information to the server
                }
                else if (rdBtnChoice2.Checked == true)
                {
                    SentBytes = Encoding.UTF8.GetBytes("2|" + timeElapsed.ToString());
                    server.Send(SentBytes);                         // send information to the server
                }
                else if (rdBtnChoice3.Checked == true)
                {
                    SentBytes = Encoding.UTF8.GetBytes("3|" + timeElapsed.ToString());
                    server.Send(SentBytes);                         // send information to the server
                }
                else if (rdBtnChoice4.Checked == true)
                {
                    SentBytes = Encoding.UTF8.GetBytes("4|" + timeElapsed.ToString());
                    Debug.WriteLine(Encoding.UTF8.GetString(SentBytes));
                    server.Send(SentBytes);                         // send information to the server
                }
                QuestionTimer.Enabled = false;
                AnswerSent = true;
            }
        }



        ///Function:		btnRightAnswer_Click
        ///Description:     this method is called when user want to view the right answer
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnRightAnswer_Click(object sender, EventArgs e)
        {
            formQA.ShowDialog();
        }



        ///Function:		btnLeaderboard_Click
        ///Description:     this method is called when user want to view the leaderboard
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void btnLeaderboard_Click(object sender, EventArgs e)
        {
            formLeaderboard.ShowDialog();
        }



        ///Function:		FormPlayer_FormClosing
        ///Description:     this method is called when user want to close the form
        ///                 this method will first show a message to check whether the user really want to quit
        ///                 if yes, the methos will close the socket and close the form
        ///                 if not, the method will allow user to continue to answer question
        ///Parameters:      object sender:  the sender of the event
        ///                 EventArgs e:    event argument
        ///Return Values:   NONE
        private void FormPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(counter==0)
            {
                e.Cancel = false;
            }
            else
            {
                // if user close the form when the user is answering the question
                if (counter <= ROUNDS)
                {
                    var result = MessageBox.Show("Are you sure you want to quit?", "SET Test", MessageBoxButtons.YesNo);

                    // if yes, close the socket and close the form
                    if (result == DialogResult.Yes)
                    {
                        SentBytes = Encoding.UTF8.GetBytes("5|0");
                        server.Send(SentBytes);
                        done = true;
                        th.Join();
                        server.Close();
                    }
                    else
                    {
                        e.Cancel = true;        // if not, allow user to continue to answer question
                    }
                }
                else
                {
                    th.Join();
                    server.Close();             // if user already finish all question, will not ask for sure
                }
            }
        }
    }
}
