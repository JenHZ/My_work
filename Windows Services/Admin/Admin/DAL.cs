/*
*Filename:		DAL.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class DAL, methods in this calss is used to talk to the database 
*               get the information of the question and choice text, and allow user to change the text
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql;
using MySql.Data;
using MySql.Web;
using MySql.Data.MySqlClient;



namespace Admin
{
    /// NAME	:	DAL
    /// PURPOSE :   This class is used to talk to the database 
    public class DAL
    {
        private MySqlConnection conn;               // used to connect to mysql
        private MySqlCommand cmd;                   // mysql command
        private MySqlDataReader rdr;                // read the result



        ///Function:		ConnectToDB
        ///Description:     this method is called when user try to connect to the database
        ///                 if it is connected, it will show a message
        ///                 and the textboxes and button used for connection will be disabled
        ///                 if not, it will display an error message indicating what is wrong
        ///Parameters:      string ipAddress: the ip address
        ///                 string userName: user name
        ///                 string passWord: password
        ///Return Values:   string result: indicate the status of the connection
        public string ConnectToDB(string ipAddress, string userName, string passWord)
        {
            string result = "";
            string myConnectionString = "";
            
            // the connection string
            myConnectionString = "server=" + ipAddress + "; user=" + userName + "; pwd=" + passWord + ";database=questions";

            try
            {
                // try to connect to the database
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                cmd = new MySqlCommand();
                cmd.Connection = conn;
            }
            catch (ArgumentException a_ex)
            {
                result = "Check the Connection String " + a_ex.Message + a_ex.ToString();
            }
            catch (MySqlException ex)
            {
                // indicate the problem of the connection
                switch (ex.Number)
                {
                    case 1042:
                        result = "Unable to connect to any of the specified MySQL hosts";
                        break;
                    case 0:
                        result = " Access denied (Check DB name,username,password)";
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
            }
            return result;                  
        }



        ///Function:		GetQuestionList
        ///Description:     this method is called to get the list of the question, including the id, and the text
        ///Parameters:      NONE
        ///Return Values:   List<string> strList: the list of the question text
        public List<string> GetQuestionList()
        {
            // the command string used to get the question text
            string sqlCmd = "SELECT QuestionText FROM Questions ORDER BY QuestionID;";
            List<string> strList = new List<string>();      // used to hold all questions
            // execute the command
            try
            {
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // read the result of the mysql command
                while (rdr.Read())
                {
                    // read the string and add to the question list
                    string str = rdr["QuestionText"].ToString();
                    strList.Add(str);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);   // in case exception happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                rdr.Close();        // close the reader
            }

            return strList;
        }




        ///Function:		GetLeaderBoard
        ///Description:     called to get the information of the leaderboard from the database
        ///Parameters:      NONE
        ///Return Values:   result: string that contain the info of the leaderboard
        public String GetLeaderBoard()
        {
            // mysql command string
            String sqlCmd = "SELECT userName, sum(score) as TotalScore " +
                            "FROM Users inner join UserScore on users.userID = userscore.userid " +
                            "GROUP BY users.userID ORDER BY TotalScore DESC;";
            String result = "";

            // try to get the command result
            try
            {
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // add "|" between each field that read from the database
                while (rdr.Read())
                {
                    result += rdr["userName"].ToString() + "|" + rdr["TotalScore"].ToString() + "|";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message); // in case something wrong
            }
            finally
            {
                if (conn.State == ConnectionState.Open) // if still open, close it
                {
                    conn.Close();
                }
                rdr.Close();            // close the reader
            }

            return result;
        }



        ///Function:		UpdateRightAnswer
        ///Description:     called to get update the right answer for a specific question
        ///Parameters:      int dif: inidcate the difference between the new answer and the previous
        ///                 string questionID: the question id
        ///Return Values:   string result: indicate the status of update
        public string UpdateRightAnswer(int dif, string questionID)
        {
            string result = "";
            
            // mysql command string
            string sqlCmd = "UPDATE Questions SET correctChoiceID = correctChoiceID + " + dif.ToString() + " WHERE QuestionID = " + questionID + ";";

            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                result = "Exception: " + ex.Message; // in case something wrong happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
                rdr.Close();                // close the reader
            }
            return result;
        }



        ///Function:		UpdateQuestion
        ///Description:     called to get update the question text for a specific question
        ///Parameters:      string question: the new answer question text
        ///                 string questionID: the question id
        ///Return Values:   string result: indicate the status of update
        public string UpdateQuestion(string question, string questionID)
        {
            string result = "";

            // mysql command string
            string sqlCmd = "UPDATE Questions SET questionText = '" + question + "' WHERE QuestionID = " + questionID + ";";

            try
            {
                // do the mysql command
                cmd.CommandText = sqlCmd;
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);        // in case something wrong happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();       // close the connection
                }
                rdr.Close();            // close the reader
            }
            return result;
        }



        ///Function:		UpdateChoice
        ///Description:     called to get update the choice text for a specific question
        ///Parameters:      string choice: the new choice text
        ///                 string questionID: the choice id
        ///Return Values:   string result: indicate the status of update
        public string UpdateChoice(string choice, string id)
        {
            string result="";

            // mysql command string
            string sqlCmd = "UPDATE Choices SET choiceText = '" + choice + "' WHERE choiceID = " + id + ";";

            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
                rdr.Close();                // close the reader
            }
            return result;
        }



        ///Function:		GetQuestionText
        ///Description:     called to get the question text for a specific question
        ///Parameters:      int whichOne: the question id
        ///Return Values:   string question: the text of the question
        public string GetQuestionText(int whichOne)
        {
            string question = "";

            // mysql command string to get the text of the question
            string sqlCmd = "SELECT QuestionText FROM Questions WHERE QuestionID=" + whichOne + ";";
            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // read the result
                while (rdr.Read())
                {
                    question = rdr["QuestionText"].ToString();
                }
            }
            catch (Exception ex)
            {
                question = "Exception: " + ex.Message;    // in case something wrong happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();       // close the connection
                }
                rdr.Close();            // close the reader
            }
            return question;
        }



        //Function:		    GetChoiceText
        ///Description:     called to get the choice text for a specific question
        ///Parameters:      List<string> choices: list of choice text
        ///                 List<string> choiceID: list of choice id
        ///                 int whichOne: the wuestion id
        ///Return Values:   string result: indicate the error message
        public string GetChoiceText(List<string> choices, List<string> choiceID, int whichOne)
        {
            string result = "";
            // mysql command string to get the text of the choices and choice ID for a specific question
            string sqlCmd = "SELECT choiceText, choiceID FROM Choices WHERE QuestionID=" + whichOne + ";";
            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // read the result
                while (rdr.Read())
                {
                    // add the result read from the database to the list
                    choices.Add(rdr["choiceText"].ToString());
                    choiceID.Add(rdr["choiceID"].ToString());
                }
            }
            catch (Exception ex)
            {
                result = "Exception: " + ex.Message;    // in case something wrong happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();                               // close the connection
                }
                rdr.Close();                                    // close the reader
            }
            return result;
        }



        //Function:		    GetCorrectAnswer
        ///Description:     called to get the correct answer for a specific question
        ///Parameters:      int whichOne: the question id
        ///Return Values:   int result: the correct answer id
        public int GetCorrectAnswer(int whichOne)
        {
            int result = 0;
            // mysql command string to get the right answer for a specific question
            string sqlCmd = "SELECT correctChoiceID FROM Questions WHERE QuestionID=" + whichOne + ";";
            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                result = (int)cmd.ExecuteScalar();              // read the result
            }
            catch (Exception ex)
            {
                //result = "Exception: " + ex.Message;    // in case something wrong happen
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();                               // close the connection
                }
                rdr.Close();                                    // close the reader
            }
            return result;
        }



        ///Function:		GetUserStatus
        ///Description:     called to get the information of the user status from the database
        ///Parameters:      NONE
        ///Return Values:   string result: the information of the status
        public String GetUserStatus()
        {
            // mysql command string
            String sqlCmd = "SELECT userName, sum(score) as Total " +
                            "FROM Users u LEFT OUTER join UserScore us on u.userID = us.userid " +
                            "WHERE alive = true GROUP BY u.userID ORDER BY Total DESC;";

            String result = "";     // used to hold the result from database

            // try to get the command result
            try
            {
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // add "|" between each field that read from the database
                while (rdr.Read())
                {
                    result += rdr["userName"].ToString() + "|" + rdr["Total"].ToString() + "|";
                }
            }
            catch (Exception ex)
            {
                result = "Exception: " + ex.Message;        // in case something wrong
            }
            finally
            {
                if (conn.State == ConnectionState.Open)             // if still open, close it
                {
                    conn.Close();
                }
                rdr.Close();        // close the reader
            }

            return result;
        }




        ///Function:		GetAllQuestions
        ///Description:     called to get the information of the question details
        ///Parameters:      NONE
        ///Return Values:   List<Questions> questionList: list of all quesitons
        public List<Questions> GetAllQuestions()
        {
            // mysql command string to get question text, question id and average time
            String sqlCmd = "SELECT q.questionID, questionText, average FROM Questions q " +
                "LEFT OUTER JOIN (SELECT questionID, (20-avg(score)) AS average " +
                "FROM userscore where score > 0 group by questionID) a on q.questionID = a.questionID " +
                "ORDER BY questionID";

            // list of all questions
            List<Questions> questionList = new List<Questions>();

            // try to get the command result
            try
            {
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                // read the result
                while (rdr.Read())
                {
                    // fill the field of the question and add to the list
                    Questions newQ = new Questions();
                    newQ.questionID = (int)rdr["questionID"];
                    newQ.questionText = (String)rdr["questionText"];
                    newQ.averageTime = rdr["average"].ToString();
                    questionList.Add(newQ);
                }
            }
            catch (Exception ex)
            {
                //Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
                rdr.Close();                // close the reader
            }

            return questionList;
        }



        ///Function:		GetPercentage
        ///Description:     called to get the information of the GetPercentage that answer correctly for each question
        ///Parameters:      NONE
        ///Return Values:   List<double> percentage: list of the percentage for all the questions
        public List<double> GetPercentage()
        {
            List<int> questionAnswered = new List<int>();       // list to store questions that has been answered correctly
            List<int> questionTotal = new List<int>();          // list to store questions
            List<double> percentage = new List<double>();       // list to store percentage for each question
            int ret;

            // mysql command string to get question count that has been answered correctly
            String sqlCmd = "SELECT q.questionID, answered FROM questions q LEFT OUTER JOIN " +
                "(SELECT questionid, count(questionid) as answered from userscore " +
                "WHERE score > 0 GROUP BY questionID) a on q.questionid = a.questionid " +
                "ORDER BY q.questionid";

            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                // read the result
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // if score is not null, that means has been answered
                    if (!rdr.IsDBNull(rdr.GetOrdinal("answered")))
                    {
                        ret = Int32.Parse(rdr["answered"].ToString());
                        questionAnswered.Add(ret);
                    }
                    else
                    {
                        questionAnswered.Add(0);        // not answered
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
                rdr.Close();                // close the reader
            }


            // mysql command string to get question count that has been answered
            sqlCmd = "SELECT q.questionID, total FROM questions q LEFT OUTER JOIN " +
                "(SELECT questionID, COUNT(questionID) as total FROM userscore " +
                "GROUP BY questionID) a on q.questionid = a.questionid ORDER BY q.questionid";

            try
            {
                // do the command
                cmd.CommandText = sqlCmd;
                conn.Open();
                // read the result
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // if score is not null, that means has been answered
                    if (!rdr.IsDBNull(rdr.GetOrdinal("total")))
                    {
                        ret = Int32.Parse(rdr["total"].ToString());
                        questionTotal.Add(ret);
                    }
                    else
                    {
                        questionTotal.Add(0);       // not answered
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();           // close the connection
                }
                rdr.Close();                // close the reader
            }


            // loop to calculate the percentage for each question
            for (int i = 0; i < questionAnswered.Count; i++)
            {
                // can not divide 0
                if (questionTotal[i] != 0)
                {
                    double percen = (double)questionAnswered[i] / questionTotal[i];
                    percentage.Add(percen);
                }
                else
                {
                    percentage.Add(0);
                }
            }

            return percentage;
        }
    }
}
