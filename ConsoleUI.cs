using Microsoft.VisualBasic.FileIO;
using System;
using System.Data.Common;
using System.Text;

namespace OthelloGame
{
    public class ConsoleUI
    {
        private char m_firstTypeOFToken = 'X';
        private char m_secondTypeOfToken = 'O';     
        
        public int GetBoardSize()
        {
            int dimension;

            Console.WriteLine("Please choose board size by entering the corresponding number:");
            Console.WriteLine("1 - 6X6");
            Console.WriteLine("2 - 8X8");
            while(!int.TryParse(Console.ReadLine(), out dimension) || (dimension != 1 && dimension != 2))
            {
                Console.WriteLine($"Invalid input. Please enter a valid choice");
            }

            int boardSize = dimension == 1 ? 6 : 8;

            return boardSize;
        }    

        public (int Row, int Col) GetPlayerMove(Player i_Player, int i_boardSize)
        {         
            Console.WriteLine(@"{0}, place your Token ({1}) by enter your move in the format 'row,column' (e.g., 2,B) or 'Q' to quit:", i_Player.PlayerName, i_Player.PlayerToken);
            string input = Console.ReadLine();

            if (input.ToUpper() == "Q")
            {
                Environment.Exit(0); // Quit the game
            }

            string[] tokens = input.Split(',');
            columnLetterToIndex colOption;

            while (tokens.Length != 2 || !int.TryParse(tokens[0], out int row) || !Enum.TryParse(tokens[1], true, out colOption) ||
                   row < 1 || row> i_boardSize ||  (int)colOption < 0 || (int)colOption > i_boardSize - 1 )
            {
                Console.WriteLine("Invalid input. Please enter your move in the correct format 'row,column' (e.g., 2,B) and in the correct range:");
                input = Console.ReadLine();
                tokens = input.Split(',');
            }

            int columnAsInt = (int)colOption;

            // Adjust for 1-based input by subtracting 1 for 0-based indexing
            return (int.Parse(tokens[0]) - 1, columnAsInt);
        }

        public void DisplayBoard(GameBoard i_Board)
        {
            char[,] board = i_Board.GetBoard();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write($"| {board[i, j]} ");
                }
                Console.WriteLine("|");
            }
        }

       




        public void DisplayWinner(string i_Winner)
        {
            Console.WriteLine($"The winner is: {i_Winner}");
        }


        public void PrintBoard(GameBoard i_board)
        {
            char[,] board = i_board.GetBoard();
            int boardWidth = board.GetLength(0);
            int boardHeight = board.GetLength(1);
            StringBuilder rowInBoard = new StringBuilder("");
            string cellInBoard;
            int rowNumber = 1;

            printABCHeaderInBoard(boardWidth);
            printLineBetweenRows(boardWidth);
            for (int i = 0; i < boardWidth; i++)
            {
                Console.Write(@"{0} |", rowNumber);
                for (int j = 0; j < boardHeight; j++)
                {
                    cellInBoard = string.Format(@" {0} |", board[i, j]);
                    rowInBoard.Append(cellInBoard);
                }

                Console.WriteLine(rowInBoard);
                rowInBoard.Clear();
                printLineBetweenRows(boardWidth);
                rowNumber++;
            }
        }

        public void printABCHeaderInBoard(int i_boardWidth)
        {
            char character = 'A';
            StringBuilder abcHeader = new StringBuilder("");

            for (int i = 0; i < i_boardWidth; i++)
            {
                abcHeader.AppendFormat("{0}   ", character);
                character++;
            }

            Console.WriteLine(@"    {0}", abcHeader);
        }

        public void printLineBetweenRows(int i_boardWidth)
        {
            StringBuilder lineHeader = new StringBuilder("");

            for (int i = 0; i < i_boardWidth; i++)
            {
                lineHeader.Append("====");
            }
            lineHeader.Append("=");

            Console.WriteLine(@"  {0}", lineHeader);
        }


        //new combine 


         public void Start()
        {
            bool playAgain = true;

            while (playAgain)
            {
                PlayGame();
                Console.WriteLine("Do you want to play again? (Y/N)");
                string response = Console.ReadLine()?.ToUpper();

                // Validate input for play again prompt
                while (response != "Y" && response != "N")
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' for Yes or 'N' for No:");
                    response = Console.ReadLine()?.ToUpper();
                }

                if (response == "N")
                {
                    playAgain = false; // If the player chooses no, exit the loop
                    Console.WriteLine("Thank you for playing! Goodbye.");
                }
            }
        }

        private void PlayGame()
        {
            Console.WriteLine("Enter the name of Player 1:");
            string player1Name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(player1Name)) // Validate Player 1's name
            {
                Console.WriteLine("Invalid input. Please enter a valid name:");
                player1Name = Console.ReadLine();
            }

            Player player1 = new Player(player1Name, m_firstTypeOFToken);

            Console.WriteLine("Do you want to play against another player (P) or the computer (C)?");
            string opponentChoice = Console.ReadLine()?.ToUpper();

            while (opponentChoice != "P" && opponentChoice != "C")
            {
                Console.WriteLine("Invalid input. Please enter 'P' for another player or 'C' for computer:");
                opponentChoice = Console.ReadLine()?.ToUpper();
            }

            AIPlayer aiPlayer = null;
            Player player2 = null;

            if (opponentChoice == "P")
            {
                Console.WriteLine("Enter the name of Player 2:");
                string player2Name = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(player2Name)) // Validate Player 2's name
                {
                    Console.WriteLine("Invalid input. Please enter a valid name:");
                    player2Name = Console.ReadLine();
                }
                player2 = new Player(player2Name, m_secondTypeOfToken);
            }
            else
            {
                aiPlayer = new AIPlayer("Computer", m_secondTypeOfToken);
            }

            int boardSize = GetBoardSize();
            GameBoard board = new GameBoard(boardSize, boardSize);

            board.InitializeBoard(m_firstTypeOFToken, m_secondTypeOfToken); 
            GameLogic game = new GameLogic(board, player1, aiPlayer == null ? player2 : new Player(aiPlayer.PlayerName, aiPlayer.PlayerToken));
            bool player1CanMove, player2CanMove;

            // 5. Main game loop
            while (!game.IsGameOver(m_firstTypeOFToken, m_secondTypeOfToken))
            {
                ClearScreen();
                PrintBoard(board);
                (int row, int col) move;
                bool validMove = false; // Flag to check for valid move

                // Check if either player can move
                player1CanMove = board.AnyValidMoves(player1.PlayerToken, m_firstTypeOFToken, m_secondTypeOfToken);
                player2CanMove = board.AnyValidMoves(aiPlayer?.PlayerToken ?? player2.PlayerToken, m_firstTypeOFToken, m_secondTypeOfToken);
                if (!player1CanMove && !player2CanMove)
                {
                    // If no player can move, the game is over
                    break;
                }

                // Handle Player 1's turn
                if (game.CurrentPlayer == player1 && player1CanMove)
                {
                    while (!validMove)
                    {
                        try
                        {
                            // Get Player 1's move and validate
                            move = GetPlayerMove(player1, boardSize);
                            game.ApplyMove(move.row, move.col, m_firstTypeOFToken, m_secondTypeOfToken);
                            Console.WriteLine($"{player1.PlayerName} made a move at ({move.row + 1},{(columnLetterToIndex)(move.col)})");
                            validMove = true; // Valid move made
                            Thread.Sleep(1000);// Pause execution for 1 second to allow the user to see the result of their move before the component responds with its move.
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message); // Show error message and prompt for another move
                        }
                    }
                }
                // Handle Player 2 or AI's turn
                else if (player2CanMove)
                {
                    while (!validMove)
                    {
                        try
                        {
                            if (aiPlayer != null)
                            {
                                // AIPlayer makes its move
                                move = aiPlayer.ChooseMove(board, m_firstTypeOFToken, m_secondTypeOfToken);
                                game.ApplyMove(move.row, move.col, m_firstTypeOFToken, m_secondTypeOfToken);
                                Console.WriteLine($"The Computer play its turn at ({move.row + 1},{(columnLetterToIndex)move.col})");
                                Thread.Sleep(1500);// Pause for 1.5 seconds to ensure the user has time to read the displayed messages before the screen is cleared.
                            }
                            else
                            {
                                // Get Player 2's move and validate
                                move = GetPlayerMove(player2, boardSize);
                                game.ApplyMove(move.row, move.col, m_firstTypeOFToken, m_secondTypeOfToken);
                                Console.WriteLine($"{player2.PlayerName} play its turn at ({move.row + 1},{(columnLetterToIndex)move.col})");
                            }
                            validMove = true; // Valid move made
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message); // Show error message and prompt for another move
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{game.CurrentPlayer.PlayerName} has no valid moves. Skipping turn.");
                }

                game.SwitchTurn();
            }
            string winner = game.GetWinner();

            DisplayWinner(winner);
        }

        private static void ClearScreen()
        {
            #if WINDOWS
                Ex02.ConsoleUtils.Screen.Clear();
            #else
                Console.Clear(); 
            #endif
        }

        public enum columnLetterToIndex
        {
            A = 0,
            B,
            C,
            D,
            E,
            F,
            G,
            H
        }
    }
}
