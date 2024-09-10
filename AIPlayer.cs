namespace OthelloGame
{
    public class AIPlayer
    {
        private readonly Random m_RandomGenerator;
        private Player m_Player;

        public AIPlayer(string i_PlayerName, char i_PlayerToken)
        {
            m_Player = new Player(i_PlayerName, i_PlayerToken);  // Use composition to hold player data
            m_RandomGenerator = new Random();
        }
        public char PlayerToken => m_Player.PlayerToken;
        public string PlayerName => m_Player.PlayerName;

        public (int Row, int Col) ChooseMove(GameBoard i_Board, char firstTypeOfToken, char secondTypeOfToken)
        {
            
            List<(int Row, int Col, int Score)> validMovesWithScores = new List<(int Row, int Col, int Score)>();

            for (int row = 0; row < i_Board.GetBoard().GetLength(0); row++)
            {
                for (int col = 0; col < i_Board.GetBoard().GetLength(1); col++)
                {
                    if (i_Board.IsMoveValid(row, col, this.PlayerToken, firstTypeOfToken, secondTypeOfToken))
                    {
                        int moveScore = i_Board.GetMoveScore(row, col, this.PlayerToken, firstTypeOfToken, secondTypeOfToken);
                        validMovesWithScores.Add((row, col, moveScore));
                    }
                }
            }

            if (validMovesWithScores.Count == 0)
            {
                throw new InvalidOperationException("No valid moves available for the AI.");
            }

            validMovesWithScores.Sort((move1, move2) => move2.Score.CompareTo(move1.Score));
            var bestMove = validMovesWithScores.FirstOrDefault();

            if (bestMove.Row >= 0 && bestMove.Row < i_Board.GetBoard().GetLength(0) && 
                bestMove.Col >= 0 && bestMove.Col < i_Board.GetBoard().GetLength(1))
            {

                return (bestMove.Row, bestMove.Col);
            }
            else
            {
                throw new InvalidOperationException("AI attempted to make a move outside the bounds of the board.");
            }
        }
    }
}