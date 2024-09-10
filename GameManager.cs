//using System;

//namespace OthelloGame
//{
//    public class GameManager
//    {


//        public void Start()
//        {
//            bool playAgain = true;
//            while (playAgain)
//            {
//                PlayGame();
//                Console.WriteLine("Do you want to play again? (Y/N)");
//                string response = Console.ReadLine()?.ToUpper();

//                // Validate input for play again prompt
//                while (response != "Y" && response != "N")
//                {
//                    Console.WriteLine("Invalid input. Please enter 'Y' for Yes or 'N' for No:");
//                    response = Console.ReadLine()?.ToUpper();
//                }

//                if (response == "N")
//                {
//                    playAgain = false; // If the player chooses no, exit the loop
//                    Console.WriteLine("Thank you for playing! Goodbye.");
//                }
//            }
//        }

//        private void PlayGame()
//        {
//            Console.WriteLine("Enter the name of Player 1:");
//            string player1Name = Console.ReadLine();
//            while (string.IsNullOrWhiteSpace(player1Name)) // Validate Player 1's name
//            {
//                Console.WriteLine("Invalid input. Please enter a valid name:");
//                player1Name = Console.ReadLine();
//            }
//            Player player1 = new Player(player1Name, 'X');

//            Console.WriteLine("Do you want to play against another player (P) or the computer (C)?");
//            string opponentChoice = Console.ReadLine()?.ToUpper();
//            while (opponentChoice != "P" && opponentChoice != "C")
//            {
//                Console.WriteLine("Invalid input. Please enter 'P' for another player or 'C' for computer:");
//                opponentChoice = Console.ReadLine()?.ToUpper();
//            }

//            AIPlayer aiPlayer = null;
//            Player player2 = null;

//            if (opponentChoice == "P")
//            {
//                Console.WriteLine("Enter the name of Player 2:");
//                string player2Name = Console.ReadLine();
//                while (string.IsNullOrWhiteSpace(player2Name)) // Validate Player 2's name
//                {
//                    Console.WriteLine("Invalid input. Please enter a valid name:");
//                    player2Name = Console.ReadLine();
//                }
//                player2 = new Player(player2Name, 'O');
//            }
//            else
//            {
//                aiPlayer = new AIPlayer("Computer", 'O');
//            }
//            var (rows, cols) = ConsoleUI.GetBoardSize();
//            GameBoard board = new GameBoard(rows, cols);
//            board.InitializeBoard(player1.PlayerToken, player2.PlayerToken); 
//            GameLogic game = new GameLogic(board, player1, aiPlayer == null ? player2 : new Player(aiPlayer.PlayerName, aiPlayer.PlayerToken));

//            bool player1CanMove, player2CanMove;

//            // 5. Main game loop
//            while (!game.IsGameOver())
//            { 
//                ClearScreen();
//                ConsoleUI.PrintBoard(board);

//                (int row, int col) move;
//                bool validMove = false; // Flag to check for valid move

//                // Check if either player can move
//                player1CanMove = board.AnyValidMoves(player1.PlayerToken);
//                player2CanMove = board.AnyValidMoves(aiPlayer?.PlayerToken ?? player2.PlayerToken);

//                if (!player1CanMove && !player2CanMove)
//                {
//                    // If no player can move, the game is over
//                    break;
//                }

//                // Handle Player 1's turn
//                if (game.CurrentPlayer == player1 && player1CanMove)
//                {
//                    while (!validMove)
//                    {
//                        try
//                        {
//                            // Get Player 1's move and validate
//                            move = ConsoleUI.GetPlayerMove(player1);
//                            game.ApplyMove(move.row, move.col);
//                            Console.WriteLine($"{player1.PlayerName} made a move at ({move.row + 1},{columnLetterToIndex(move.col)})");
//                            validMove = true; // Valid move made
//                        }
//                        catch (InvalidOperationException ex)
//                        {
//                            Console.WriteLine(ex.Message); // Show error message and prompt for another move
//                        }
//                    }
//                }
//                // Handle Player 2 or AI's turn
//                else if (player2CanMove)
//                {
//                    while (!validMove)
//                    {
//                        try
//                        {
//                            if (aiPlayer != null)
//                            {
//                                // AIPlayer makes its move
//                                move = aiPlayer.ChooseMove(board);
//                                game.ApplyMove(move.row, move.col);
//                                Console.WriteLine($"The Computer played its turn at ({move.row + 1},{move.col + 1})");
//                            }
//                            else
//                            {
//                                // Get Player 2's move and validate
//                                move = ConsoleUI.GetPlayerMove(player2);
//                                game.ApplyMove(move.row, move.col);
//                                Console.WriteLine($"{player2.PlayerName} made a move at ({move.row + 1},{move.col + 1})");
//                            }
//                            validMove = true; // Valid move made
//                        }
//                        catch (InvalidOperationException ex)
//                        {
//                            Console.WriteLine(ex.Message); // Show error message and prompt for another move
//                        }
//                    }
//                }
//                else
//                {
//                    Console.WriteLine($"{game.CurrentPlayer.PlayerName} has no valid moves. Skipping turn.");
//                }
//                game.SwitchTurn();
//            }
//            string winner = game.GetWinner();
//            ConsoleUI.DisplayWinner(winner);
//        }

//        private static void ClearScreen()
//        {
//            #if WINDOWS
//                Ex02.ConsoleUtils.Screen.Clear();
//            #else
//                Console.Clear(); 
//            #endif
//        }
//    }
//}
