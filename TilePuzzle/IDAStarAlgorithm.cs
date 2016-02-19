/*
*Filename:		IDAStarAlgorithm.cs
*Project:		WMP Final Project
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.12.11
*Description:	This file contain a class called IDAStarAlgorithm, it contains the methods to calculate the solution for the game
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePuzzle
{
    class IDAStarAlgorithm
    { 
		// represent the left, up, right, and down operation
        private int[] up = { -1, 0 };
        private int[] down = { 1, 0 };
        private int[] left = { 0, -1 };
        private int[] right = { 0, 1 };

        // used to represent the direction
        private const int UP = 0;
        private const int DOWN = 2;
        private const int LEFT = 1;
        private const int RIGHT = 3;
        //private int[,] tState;
        private int[,] sState;

        private int SIZE;

        //the point of the target
        private int[,] targetPoints;

        // used to store the direction of each step for the solution
        public int[] moves = new int[100000];
        //public List<int> moves = new List<int>();

        public long ans = 0; //current idea cost


       // the target array
        private int[,] tState = {
               {1 ,2 ,3 ,4 } ,
               {5 ,6 ,7 ,8 } ,
               {9 ,10,11,12} ,
               {13,14,15,0 }
           };

        // used to represent the row and column of the blank
        private static int blank_row, blank_column;

		
		
		///Function:		IDAStarAlgorithm -- constructor
        ///Description:     to instantiate a IDAStarAlgorithm instance
        ///Parameters:      int[,] state: the two dimentional array that want to get the solution
        ///Return Values:   NONE
        public IDAStarAlgorithm(int[,] state)
        {
            SIZE = 4;
            targetPoints = new int[16,2];			// used to store the target points
         
			
            this.sState = state;
            
			// loop to get the position of 0
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (state[i,j] == 0)
                    {
                        blank_row = i;			// record the row and column of the point 0
                        blank_column = j;
                        break;
                    }
                }
            }

            //to get the target array
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    targetPoints[tState[i,j],0] = i; //the information of row

                    targetPoints[tState[i,j],1] = j; //the information of column
                }
            }
        }

        
		
		///Function:		canSolve
        ///Description:     called to check whether a given array can be solved or not
        ///Parameters:      int[,] state: the two dimentional array that want to get the solution
        ///Return Values:   bool true indicates the array can be solved
		///						 false: indicate the array can not be solved
        public bool canSolve(int[,] state)
        {
            // if the size of the array is an odd number
			if (SIZE % 2 == 1)
            { 
                return (getInversions(state) % 2 == 0);
            }
            else
            { // if the size of the array is a even number
                if(blank_row % 2 == 1) 
                { //if the blank is at a odd row
                    return (getInversions(state) % 2 == 0);
                } 
                else 
                { //if the blank is at a even row
                    return (getInversions(state) % 2 == 1);
                }
                
            }
        }





        ///Function:		solve
        ///Description:     called to calculate the solution for the array 
        ///                 this is a recursive method that will call itself until the problem is solved
        ///Parameters:      int[,] state: the two dimentional array that want to get the solution
        ///                 int blank_row: the row of the blank point
        ///                 int blank_column: the column of the blank point
        ///                 int dep: the current depth of the recursive
        ///                 long d: the direction of last move
        ///                 long h: the current estimate of the current state
        ///Return Values:   bool true indicates the array can be solved
        ///			
        public bool solve(int[,] state, int blank_row, int blank_column, int dep, long d, long h)
        {
            long h1;
            bool isSolved = true;

            // compare with the target array to check whether it is solved
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (state[i,j] != tState[i,j])
                    {
                        isSolved = false;           // not solved
                    }
                }
            }
            if (isSolved)
            {
                return true;        // return solved
            }
            if (dep == ans)         // if already reach the steps that for this level of recursive
            {
                return false;
            }

            //the row and column of the point 0 after it move
            int blank_row1 = blank_row;
            int blank_column1 = blank_column;
            int[,] state2 = new int[4,4];       // used to copy the information of the current array

            // loop the four direstions
            for (int direction = 0; direction < 4; direction++)
            {
                // copy the begining state to the state2
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        state2[i,j] = state[i,j];
                    }
                }

                // if this movement is just opposite to last movement, just skip it
                if (direction != d && (d % 2 == direction % 2))
                {
                    continue;
                }

                // if move up
                if (direction == UP)
                {
                    blank_row1 = blank_row + up[0];         // new position of point 0
                    blank_column1 = blank_column + up[1];
                }
                else if (direction == DOWN)         // if move down
                {
                    blank_row1 = blank_row + down[0];           // new position of point 0
                    blank_column1 = blank_column + down[1];
                }
                else if (direction == LEFT)         // if move to left
                {
                    blank_row1 = blank_row + left[0];           // new position of point 0
                    blank_column1 = blank_column + left[1];
                }
                else
                {
                    blank_row1 = blank_row + right[0];          // new position of point 0
                    blank_column1 = blank_column + right[1];
                }

                // need to check the boundery
                if (blank_column1 < 0 || blank_column1 == SIZE || blank_row1 < 0 || blank_row1 == SIZE)
                {
                    continue;
                }

                // switch the number that the point 0 moved to with 0
                state2[blank_row,blank_column] = state2[blank_row1,blank_column1];
                state2[blank_row1,blank_column1] = 0;

                // check if the current state is closer to the target
                if (direction == DOWN && blank_row1 > targetPoints[state[blank_row1,blank_column1],0])
                {
                    h1 = h - 1;
                }
                else if (direction == UP && blank_row1 < targetPoints[state[blank_row1,blank_column1],0])
                {
                    h1 = h - 1;
                }
                else if (direction == RIGHT && blank_column1 > targetPoints[state[blank_row1,blank_column1],1])
                {
                    h1 = h - 1;
                }
                else if (direction == LEFT && blank_column1 < targetPoints[state[blank_row1,blank_column1],1])
                {
                    h1 = h - 1;
                }
                else
                {
                    // any situation that cause the estimate of current situation increased
                    h1 = h + 1;
                }

                // if the the total depth exceeded, no futher more
                if (h1 + dep + 1 > ans)
                { 
                    continue;
                }

                // assign the direction to the movement list
                moves[dep] = direction;
               
                // go to the next step
                if (solve(state2, blank_row1, blank_column1, dep + 1, direction, h1))
                {
                    return true;
                }
            }
            // if reach here, it means in the current depth (ans), can't find a solution
            // need increment ans
            return false;
        }





        ///Function:		getHeuristic
        ///Description:     called to calculate the distance between the target position and the current position for each number
        ///Parameters:      int[,] state: the two dimentional array that want to get the solution
        ///Return Values:   int heuristic: indicates the distance
        ///			
        public int getHeuristic(int[,] state)
        {
            int heuristic = 0;

            // loop to travesal all the numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (state[i, j] != 0)
                    {
                        // 	targetPoints[state[i][j]][0] , since state[i][j] is the number value in (i,j) of puzzle source state,  
                        //	targetPoints[state[i][j]] will get the target location of this number, where targetPoints[state[i][j]][0] is the row location, and targetPoints[state[i][j]][1] is the column location 	
                        heuristic = heuristic + Math.Abs(targetPoints[state[i, j], 0] - i) + Math.Abs(targetPoints[state[i, j], 1] - j);
                    }
                }
            }
            return heuristic;
        }

        


        ///Function:		getInversions
        ///Description:     called to calculate the number of pairs of inversed number in the array
        ///Parameters:      int[,] state: the two dimentional array that want to get the solution
        ///Return Values:   int inversion: indicates the number of inversed number pairs
        ///		
        public int getInversions(int[,] state)
        {
            int inversion = 0;
            int temp = 0;

            // loop to travesal all the numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = i * 4 + j + 1;
                    // travesal all the numbers that follow the current number in the array
                    while (index < (4 * 4))
                    {
                        // if the number is less than the current number, a pair of inversion found
                        if (state[index / 4,index % 4] != 0 && state[index / 4,index % 4] < state[i,j])
                        {
                            temp++;
                        }
                        index++;
                    }
                    inversion = temp + inversion;
                    temp = 0;
                }
            }
            return inversion;
        }




        ///Function:		zeroLocate
        ///Description:     called to get the position of 0 in the array
        ///Parameters:      NONE
        ///Return Values:   List<int> point: row and column of the 0 point
        ///		
		public List<int> zeroLocate()
		{
            List<int> point = new List<int>();

            // travesal all the point in the array to find 0
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
		            if(sState[i,j]==0)
                    {
                        point.Add(i);       // record the value for row and column
                        point.Add(j);
                        return point;
                    }
				}
			}

            return null;
		}





        ///Function:		solvePuzzle
        ///Description:     called to solve the puzzle
        ///Parameters:      NONE
        ///Return Values:   bool: true indicate the puzzle can be solved
        ///                       false indicate the puzzle can not be solved
        ///		
        public bool solvePuzzle()
        {
            if (canSolve(sState))
            {
                // calulate the Heuristic number and used it as the minimal estimate 
                int j = getHeuristic(sState);
                int i = -1;                 //the default position
                
                List<int> retuenRet = zeroLocate();

                // grsdually incresse the step to find the soution
                for (ans = j; ; ans++)
                {

                    if (solve(sState, retuenRet[0], retuenRet[1], 0, i, j))
                    {
                        break;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
