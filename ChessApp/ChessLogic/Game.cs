using ChessApp.ChessLogic.Enums;
using System;

namespace ChessApp.ChessLogic
{
    public class Game
    {
        private Player[] players;
        public Board Board { get; private set; }
        public Player CurrentTurn { get; private set; }
        private GameStatus Status { get; set; }
        private List<Move> movesPlayed;

        public Game(Player p1, Player p2)
        {
            players = new Player[2];
            players[0] = p1;
            players[1] = p2;
            CurrentTurn = p1;

            Board = new Board();

            movesPlayed = new List<Move>();
        }

        public bool isEnd()
        {
            return Status != GameStatus.ACTIVE;
        }

        public bool MakeMoveWithCoordinates(Player player, int fromX, int fromY, int toX, int toY)
        {
            var start = Board.Squares[fromX, fromY];
            var end = Board.Squares[toX, toY];
            var move = new Move(start, end);
            return MakeMove(player, move);
        }

        public bool MakeMove(Player player, Move move)
        {
            Piece movingPiece = move.Start.Piece;

            if (player != players[0] && player != players[1])
            {
                Console.WriteLine("Error! Someone else is trying to help!");
                return false;
            }

            if (CurrentTurn != player)
            {
                Console.WriteLine("Error! Not your turn!");
                return false;
            }

            if (movingPiece == null)
            {
                return false;
            }

            // check for valid move 

            // if there was a piece in end square, set it as killed and assign to the move as killed piece. 
            Piece destPiece = move.End.Piece;
            if (destPiece != null) //Never Null fix
            {
                destPiece.SetKilled();
                move.PieceKilled = destPiece;
            }

            // store the move 
            movesPlayed.Add(move);

            // move piece from the stat box to end box 
            move.End.Piece = movingPiece;
            move.PieceMoved = movingPiece;
            move.Start.Piece = null;

            // set the current turn to the other player 
            if (CurrentTurn == players[0])
            {
                CurrentTurn = players[1];
            }
            else
            {
                CurrentTurn = players[0];
            }

            return true;
        }

        public string PrintBoard()
        {
            return Board.ToString();
        }

    }
}
