/*
*Filename:		DAL.cs
*Project:		Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	This file contains code for DAL class 
*/



using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace myService
{
    /// NAME	:	TakeCareClient
    /// PURPOSE :   This class is used for the service to communicate with database
    class DAL
    {
        MySqlConnection conn;            // a data member to hold the connection
        MySqlCommand cmd;                // used to get and execute queries
        MySqlDataReader rdr;             // used to read multiple rows from database
        String connectionString =        // information used to connect to the database
            "server=localhost;user id=root; password = root; database=questions";



        ///Function:		DAL -- constructor
        ///Description:     to instantiate a DAL object and initialize some attributes.
        ///Parameters:      NONE
        ///Return Values:   NONE
        public DAL()
        {
            conn = new MySqlConnection(connectionString);
            cmd = new MySqlCommand();
            cmd.Connection = conn;
        }



        ///Function:		AddUser
        ///Description:     add a new user into database
        ///Parameters:      Users user:   the information of this user
        ///Return Values:   int:          the ID of this user
        public int AddUser(Users user)
        {
            // generate a query string 
            String sqlCmd = "INSERT INTO Users (userName, alive)"
                + "VALUES('"
                + user.UserName + "', '"
                + user.Status + "');";
            int userID = 0;

            try
            {
                // execute this query and get the result
                cmd.CommandText = sqlCmd;
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                userID = (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return userID;
        }



        ///Function:		UpdateUser
        ///Description:     change the user status when he/her finishes the game
        ///Parameters:      int  userID:   which user left
        ///Return Values:   NONE
        public void UpdateUser(int userID)
        {
            // generate a query string 
            String sqlCmd = "UPDATE Users SET alive = 0 WHERE userid = " + userID + ";";
            Logger.Log(sqlCmd);

            // create new connections for this query.
            MySqlConnection newConn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = newConn;
            
            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                newConn.Open();
                int result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (newConn.State == ConnectionState.Open)
                {
                    newConn.Close();
                }
            }
        }



        ///Function:		AddUserScore
        ///Description:     put a set of user score into database
        ///Parameters:      UserChoice  userChoice:   user score of a certain question
        ///Return Values:   NONE
        public void AddUserScore(UserChoice userChoice)
        {
            // generate a query string 
            String sqlCmd = "INSERT INTO UserScore (userID, questionID, score)"
                + "VALUES('"
                + userChoice.UserID + "', '"
                + userChoice.QuestionID + "', '"
                + userChoice.Score + "');";
            Logger.Log(sqlCmd);

            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }



        ///Function:		GetAllQuestions
        ///Description:     get all questions from database
        ///Parameters:      NONE
        ///Return Values:   List<Questions>:    a list of Questions objects
        public List<Questions> GetAllQuestions()
        {
            // generate a query string 
            String sqlCmd = "SELECT questionID, questionText, correctChoiceID FROM Questions";
            List<Questions> questionList = new List<Questions>();

            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();

                // a while loop to get all rows
                while (rdr.Read())
                {
                    // assign all data from database into a Questions object and then add into list
                    Questions newQ = new Questions();
                    newQ.QuestionID = (int)rdr["questionID"];
                    newQ.QuestionText = (String)rdr["questionText"];
                    newQ.CorrectChoiceID = (int)rdr["correctChoiceID"];
                    questionList.Add(newQ);
                } // end while loop
            }

            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    
                }
                rdr.Close();
            }

            return questionList;
        }



        ///Function:		GetAllChoices
        ///Description:     get all choices from database
        ///Parameters:      NONE
        ///Return Values:   List<Choices>:    a list of Choices objects
        public List<Choices> GetAllChoices()
        {
            // generate a query string 
            String sqlCmd = "SELECT choiceID, choiceText, questionID FROM choices ORDER BY questionID, choiceID";
            List<Choices> choiceList = new List<Choices>();

            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();

                // a while loop to get all rows
                while (rdr.Read())
                {
                    // assign all data from database into a Choices object and then add into list
                    Choices newC = new Choices();
                    newC.ChoiceID = (int)rdr["choiceID"];
                    newC.ChoiceText = (String)rdr["choiceText"];
                    newC.QuestionID = (int)rdr["questionID"];
                    choiceList.Add(newC);
                }// end while loop
            }

            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                   
                }
                rdr.Close();
            }

            return choiceList;
        }



        ///Function:		GetTotalScoreForSingleUser
        ///Description:     get total score for a certain user
        ///Parameters:      int   userid: which user
        ///Return Values:   int         : the score of this user
        public int GetTotalScoreForSingleUser(int userid)
        {
            // generate a query string
            String sqlCmd = "SELECT sum(score) FROM userScore where userID = " + userid + ";";
            int result = 0;
            Logger.Log(sqlCmd);

            // create new connections for this query.
            MySqlConnection newConn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = newConn;

            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                newConn.Open();

                // cast the result to an int
                Object obj = cmd.ExecuteScalar();
                if(obj != null)
                {
                    result = Convert.ToInt32(obj);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (newConn.State == ConnectionState.Open)
                {
                    newConn.Close();
                }
            }

            return result;
        }


        /*
        public List<Users> GetAllUsers()
        {
            String sqlCmd = "SELECT userID, userName, alive FROM users;";
            List<Users> userList = new List<Users>();

            try
            {
                cmd.CommandText = sqlCmd;
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Users newUser = new Users();
                    newUser.UserID = (int)rdr["userID"];
                    newUser.UserName = rdr["userName"].ToString();
                    newUser.Status = (int)rdr["alive"];
                    userList.Add(newUser);
                }
            }

            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                rdr.Close();
            }

            return userList;
        }
        */



        ///Function:		GetLeaderBoard
        ///Description:     get the ranking of all users
        ///Parameters:      NONE
        ///Return Values:   String : formatted string contains all users and their total score
        public String GetLeaderBoard()
        {
            // generate a query string
            String sqlCmd = "SELECT userName, sum(score) as TotalScore " + 
                            "FROM Users inner join UserScore on users.userID = userscore.userid " +
                            "GROUP BY users.userID ORDER BY TotalScore DESC;";
            String result = "";
            Logger.Log(sqlCmd);

            // create new connections for this query.
            MySqlConnection newConn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = newConn;

            try
            {
                // execute this query
                cmd.CommandText = sqlCmd;
                newConn.Open();
                rdr = cmd.ExecuteReader();

                // while loop to get all information and stuff them into a formatted string
                while (rdr.Read())
                {
                    result += rdr["userName"].ToString() + "|" + rdr["TotalScore"].ToString() + "|";
                } // end of while loop
            }

            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
            }
            finally
            {
                // clean up
                if (newConn.State == ConnectionState.Open)
                {
                    newConn.Close();
                    
                }
                rdr.Close();
            }

            Logger.Log(result);
            return result;
        }
    }
}
