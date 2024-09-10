using System;

namespace OthelloGame
{
    public class GameBoard
    {
        private char[,] m_Board;
        private readonly int m_Rows;
        private readonly int m_Cols;
        private readonly int[] directions = { -1, 0, 1 }; // To check in all directions

        public GameBoard(int i_Rows, int i_Cols)
        {
            m_Rows = i_Rows;
            m_Cols = i_Cols;
            m_Board = new char[m_Rows, m_Cols];
            //InitializeBoard();
        }
        
        public int Rows
        {
            get { return m_Rows; }
        }

        public int Cols
        {
            get { return m_Cols; }
        }

        public void InitializeBoard(char i_tokenPlayer1, char i_tokenPlayer2)
        {
            for (int i = 0; i < m_Rows; i++)
            {
                for (int j = 0; j < m_Cols; j++)
                {
                    m_Board[i, j] = ' '; // Empty value
                }
            }

            // Initialize 4 tokens in the center
            int middleRow = m_Rows / 2 - 1;
            int middleCol = m_Cols / 2 - 1;
            m_Board[middleRow, middleCol] = i_tokenPlayer2;
            m_Board[middleRow + 1, middleCol + 1] = i_tokenPlayer2;
            m_Board[middleRow, middleCol + 1] = i_tokenPlayer1;
            m_Board[middleRow + 1, middleCol] = i_tokenPlayer1;
        }

        public char[,] GetBoard()
        {
            return m_Board;
        }

        public bool IsMoveValid(int i_Row, int i_Col, char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken)
        {
            if (i_Row < 0 || i_Row >= m_Rows || i_Col < 0 || i_Col >= m_Cols)
            {
                return false; // Out of bounds
            }

            if (m_Board[i_Row, i_Col] != ' ')
            {
                return false; // Space is already occupied
            }

            foreach (int dRow in directions)
            {
                foreach (int dCol in directions)
                {
                    if (dRow == 0 && dCol == 0) continue; // Skip the case where both are 0 (current cell)
                    if (isBlockingOpponent(i_Row, i_Col, dRow, dCol, i_PlayerToken, firstTypeOfToken, secondTypeOfToken))
                    {
                        return true; // Valid move if it can flip opponent's tokens
                    }
                }
            }

            return false;
        }

        private bool isBlockingOpponent(int i_Row, int i_Col, int dRow, int dCol, char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken) 
        {
            int row = i_Row + dRow;
            int col = i_Col + dCol;
            char opponentToken = (i_PlayerToken == firstTypeOfToken) ? secondTypeOfToken : firstTypeOfToken;
            bool foundOpponent = false;

            // Traverse in the given direction (dRow, dCol) to look for opponent's tokens.
            while (row >= 0 && row < m_Rows && col >= 0 && col < m_Cols) 
            {
                if (m_Board[row, col] == opponentToken) 
                {
                    // Found at least one opponent's token in the direction
                    foundOpponent = true;
                } 
                else if (m_Board[row, col] == i_PlayerToken && foundOpponent) 
                {
                    // Found player's token after opponent's token(s), valid outflank
                    return true;
                } 
                else 
                {
                    // Either an empty space or the player's own token without first finding an opponent's token
                    return false;
                }

                // Move to the next cell in the direction
                row += dRow;
                col += dCol;
            }

            // Reached the edge of the board without outflanking
            return false;
        }


        public void ApplyMove(int i_Row, int i_Col, char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken)
        {
            if (i_Row < 0 || i_Row >= m_Board.GetLength(0) || i_Col < 0 || i_Col >= m_Board.GetLength(1))
            {
                throw new IndexOutOfRangeException("Attempted to apply a move outside the bounds of the board.");
            }

            // Place the player's token on the board
            m_Board[i_Row, i_Col] = i_PlayerToken;

            // Flip opponent tokens in all valid directions
            foreach (int dRow in directions)
            {
                foreach (int dCol in directions)
                {
                    if (dRow == 0 && dCol == 0) continue;

                    if (isBlockingOpponent(i_Row, i_Col, dRow, dCol, i_PlayerToken, firstTypeOfToken, secondTypeOfToken))
                    {
                        flipOpponentTokens(i_Row, i_Col, dRow, dCol, i_PlayerToken, firstTypeOfToken, secondTypeOfToken);
                    }
                }
            }
        }
        
        public int GetMoveScore(int i_Row, int i_Col, char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken)
        {
            int score = 0;
            char opponentToken = (i_PlayerToken == firstTypeOfToken) ? secondTypeOfToken : firstTypeOfToken;

            // Check in all directions for possible flips
            foreach (int dRow in directions)
            {
                foreach (int dCol in directions)
                {
                    if (dRow == 0 && dCol == 0) continue;

                    int row = i_Row + dRow;
                    int col = i_Col + dCol;
                    int currentScore = 0;

                    // If we find opponent tokens in that direction
                    while (row >= 0 && row < m_Rows && col >= 0 && col < m_Cols && m_Board[row, col] == opponentToken)
                    {
                        currentScore++;
                        row += dRow;
                        col += dCol;
                    }

                    // If the line ends with the player's own token, add to the score
                    if (row >= 0 && row < m_Rows && col >= 0 && col < m_Cols && m_Board[row, col] == i_PlayerToken)
                    {
                        score += currentScore;
                    }
                }
            }

            return score;
        }


        private void flipOpponentTokens(int i_Row, int i_Col, int dRow, int dCol, char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken)
        {
            int row = i_Row + dRow;
            int col = i_Col + dCol;

            char opponentToken = (i_PlayerToken == firstTypeOfToken) ? secondTypeOfToken : firstTypeOfToken;

            while (row >= 0 && row < m_Rows && col >= 0 && col < m_Cols && m_Board[row, col] == opponentToken)
            {
                m_Board[row, col] = i_PlayerToken; // Flip the token
                row += dRow;
                col += dCol;
            }
        }

        public bool AnyValidMoves(char i_PlayerToken, char firstTypeOfToken, char secondTypeOfToken)
        {
            for (int i = 0; i < m_Rows; i++)
            {
                for (int j = 0; j < m_Cols; j++)
                {
                    if (IsMoveValid(i, j, i_PlayerToken, firstTypeOfToken, secondTypeOfToken))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int CountTokens(char i_PlayerToken)
        {
            int count = 0;
            for (int i = 0; i < m_Rows; i++)
            {
                for (int j = 0; j < m_Cols; j++)
                {
                    if (m_Board[i, j] == i_PlayerToken)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
