/*
*Filename:		Tile.cs
*Project:		WMP Final Project
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.12.11
*Description:	This file contain a class called Tile, it contains the methods to assign the list of the random assigned list of tiles for the game,
*               the methods to decide whether the tile can move, 
*               and method to decide whether the a tile can move depending on the postion and the direction it want to move
*               and also method to decide whether all the player win
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzle
{
    class Tile
    {
        // the array of number that used to random and assign the tiles
        int[] arrange = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0};
        int[,] target = new int[4, 4] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 0 } };     // target array
        int[,] current = new int[6, 6];                 // array used to process the game, add margin to prevent out of range




        ///Function:		GetArray
        ///Description:     called to create a randomised list of number that will be used to assign the tiles
        ///                 After ramdomised the list, it will alos make sure the game can be solved
        ///Parameters:      NONE
        ///Return Values:   int[] arrange: the list of the number that will be used to assign the tiles
        public int[] GetArray()
        {
            Random rd = new Random();               // used to create random number

			// 	first the right order
            for (int i = 0; i < 15; i++)
            {
                arrange[i] = i + 1;
            }

			// loop 16 times to swithch the numbers
            for (int i = 0; i < 14; i++)
            {
                int index = rd.Next(0, 15);				// create a number between 0 and 15
                // make sure it not switch with itself
				while(index == i)
                {
                    index = rd.Next(0, 15);			// if the same, create another number
                }

                int temp = arrange[i];				// swithch the number
                arrange[i] = arrange[index];
                arrange[index] = temp;
            }

            arrange[15] = 0;				// the last number need to be 0

            SetCurrentArray();				// set the number in the array to the 6 size array as well

            return arrange;
        }


		
		
		///Function:		CheckMove
        ///Description:     called to check whether the tile can move depending on the row and column number that passed in
		///					If the tile can move, it will return which direction it can move
        ///                 After ramdomised the list, it will alos make sure the game can be solved
        ///Parameters:      int x: the row number of the tile
		///					int y: the column number of the tile
        ///Return Values:   string direction: indicate whether the tile can move and which direction and whether will win after this move
		///					
        public string CheckMove(int x, int y)
        {
            string direction = "O";					// the default direction
            
			// check if the tile above the tile is 0
            if(current[x, y + 1]==0)
            {
                direction = "U";				// up direction
                current[x, y + 1] = current[x + 1, y + 1];		// switch the tiles
                current[x + 1, y + 1] = 0;
            }
            else if (current[x + 2, y + 1] == 0)			// check if the tile below the tile is 0
            {
                direction = "D";							// down direction
                current[x + 2, y + 1] = current[x + 1, y + 1];		// switch the tiles
                current[x + 1, y + 1] = 0;
            }
            else if (current[x + 1, y ] == 0)				// check if the tile left the tile is 0
            {
                direction = "L";							// left direction
                current[x + 1, y] = current[x + 1, y + 1];	// switch the tiles
                current[x + 1, y + 1] = 0;
            }
            else if (current[x + 1, y + 2] == 0)			// check if the tile right the tile is 0
            {
                direction = "R";							// right direction
                current[x + 1, y + 2] = current[x + 1, y + 1];	// switch the tiles
                current[x + 1, y + 1] = 0;
            }

			// check if it will win after this move
            if (CheckWin())
            {
                direction += "W";			// if will win, tell 
            }

            return direction;
        }


		
		///Function:		CheckMove
        ///Description:     called to check whether the tile can move depending on the row and column number, and the direction that passed in
		///					If the tile can move, it will return it can move
        ///Parameters:      int x: the row number of the tile
		///					int y: the column number of the tile
		///					char direction: the direction the tile want to move
        ///Return Values:   int status: 0: can not move, 1: can move, 2: can move and will win after this move
		///			
        public int CheckMove(int x, int y, char direction)
        {
            int status = 0;             // 0: can not move, 1: can move, 2: can move and will win after this move
						
			// according to the direction
            switch (direction)
            {
                case 'L':			// left direction
                    if (current[x + 1, y] == 0)				// check can move
                    {
                        status = 1;

                        current[x + 1, y] = current[x + 1, y + 1];			// switch the tiles
                        current[x + 1, y + 1] = 0;
                    }
                    break;
                case 'R':			// right direction
                    if (current[x + 1, y + 2] == 0)			// check can move
                    {
                        status = 1;

                        current[x + 1, y + 2] = current[x + 1, y + 1];		// switch the tiles
                        current[x + 1, y + 1] = 0;
                    }
                    break;
                case 'U':			// up direction
                    if (current[x, y + 1] == 0)				// check can move
                    {
                        status = 1;

                        current[x, y + 1] = current[x + 1, y + 1];			// switch the tiles
                        current[x + 1, y + 1] = 0;
                    }
                    break;
                case 'D':			// down direction
                    if (current[x + 2, y + 1] == 0)			// check can move
                    {
                        status = 1;

                        current[x + 2, y + 1] = current[x + 1, y + 1];		// switch the tiles
                        current[x + 1, y + 1] = 0;
                    }
                    break;
                default:
                    break;
            }
			
			// chekc if it can win
            if (CheckWin())
            {
                status = 2;
            }

            return status;
        }



		///Function:		CheckMove
        ///Description:     called to check whether the tile can move depending on the direction that passed in
		///					It will search which tile can move and tell the direction and whether can win after this move
        ///Parameters:      char direction: the direction the tile want to move			
        ///Return Values:   List<int> result: indicate which tile can move and the direction
		///			
        public List<int> CheckMove(char direction)
        {
            // three int, the first int represent the row, the second int represetn column, 
            //  the third int indicate win (0 not win, 1 win)
            // empty list indicate can not move
            List<int> result = new List<int>();
			
			// first find which one can move
            result = FindWhichToMove(direction);
			
			// some tile can move
            if(result.Count!=0)
            {
                // check whethe can win after this move
				if (CheckWin())
                {
                    result.Add(1);
                }
            }
            return result;
        }


		
		
		///Function:		FindWhichToMove
        ///Description:     called to find out which tile can move 
        ///Parameters:      char direction: the direction the tile want to move			
        ///Return Values:   List<int> whichOne: indicate the new row and column and the index of the tile in the original list of the tile
        private List<int> FindWhichToMove(char direction)
        {
            int[] zero = new int[2];				// the position of 0
            List<int> whichOne = new List<int>();	// the list of number of new row and column a the tile that can move
			
			// loop to find the 0
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if(current[i,j]==0)
                    {
                        zero[0] = i;
                        zero[1] = j;
                        break;
                    }
                }
            }

			// according to the direction
            switch(direction)
            {
                case 'L':				// left direction
                    if(zero[1] != 4)
                    {
                        whichOne.Add(zero[0] - 1);		// the new position for the tile that can move
                        whichOne.Add(zero[1]);
                        current[zero[0], zero[1]] = current[zero[0], zero[1] + 1];		// switch the tiles
                        current[zero[0], zero[1] + 1] = 0;
                    }
                    break;
                case 'R':				// right direction
                    if (zero[1] != 1)
                    {
                        whichOne.Add(zero[0] - 1);		// the new position for the tile that can move
                        whichOne.Add(zero[1] - 2);
                        current[zero[0], zero[1]] = current[zero[0], zero[1] - 1];		// switch the tiles
                        current[zero[0], zero[1] - 1] = 0;
                    }
                    break;
                case 'U':				// up direction
                    if (zero[0] != 4)
                    {
                        whichOne.Add(zero[0]);			// the new position for the tile that can move
                        whichOne.Add(zero[1] - 1);
                        current[zero[0], zero[1]] = current[zero[0] + 1, zero[1]];		// switch the tiles
                        current[zero[0] + 1, zero[1]] = 0;
                    }
                    break;
                case 'D':				// down direction
                    if (zero[0] != 1)
                    {	
                        whichOne.Add(zero[0] - 2);		// the new position for the tile that can move
                        whichOne.Add(zero[1] - 1);
                        current[zero[0], zero[1]] = current[zero[0] - 1, zero[1]];		// switch the tiles
                        current[zero[0] - 1, zero[1]] = 0;
                    }
                    break;
                default:
                    break;

            }

            return whichOne;
        }



		///Function:		CheckWin
        ///Description:     called to check whether the game is win
        ///Parameters:      NONE
        ///Return Values:   bool status: indicate whether the game is win
        public bool CheckWin()
        {
            bool status = true;				// indicate whether can win
			
			// loop to compare the current array with the target array
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // if not the same, not win
					if (current[i+1, j+1] != target[i, j])
                    {
                        status = false;
                        break;
                    }
                }
                if (!status)		// if not the same, not win
                {
                    break;
                }
            }

            return status;
        }


		
		///Function:		GetCurrentMove
        ///Description:     called to set a string that contain the current information of the game that will be stored in the file
        ///Parameters:      NONE
        ///Return Values:   bool status: indicate whether the game is win
        public string GetCurrentMove()
        {
            String result = "";
			
			// loop to get the numbers in the array
            for(int i = 1; i< 5; i++)
            {
                for(int j = 1; j< 5; j++)
                {
                    result += "" + current[i,j]  + "|";		// add to the string
                }
            }

            return result;
        }

		
		
		///Function:		SetCurrentMove
        ///Description:     called to get a string that contain the current information of the game from a file
        ///Parameters:      string array: the string that contain the information
        ///Return Values:   NONE
        public void SetCurrentMove(string array)
        {
            string[] arrList = array.Split('|');		// parse the string
			
			// loop to get the information
            for (int i = 0; i < 16; i++)
            {
                arrange[i] = Int32.Parse(arrList[i]);
            }

            SetCurrentArray();				// assign to the current array
        }

		
		
		///Function:		SetCurrentArray
        ///Description:     called to set the current array using the information that stored in a file
        ///Parameters:      NONE
        ///Return Values:   NONE
        public void SetCurrentArray()
        {
            int counter = 0;
			
			// loop to assign the first row of the array to 1
            for (int i = 0; i < 6; i++)
            {
                current[0, i] = 1;
            }

			// loop to assign the number to the array
            for (int j = 1; j < 5; j++)
            {
                current[j, 0] = 1;

                for (int i = 1; i < 5; i++)
                {
                    current[j, i] = arrange[counter];
                    counter++;
                }
                current[j, 5] = 1;
            }

			// loop to assign the last row of the array to 1
            for (int i = 0; i < 6; i++)
            {
                current[5, i] = 1;
            }
        }
        
    }
}
