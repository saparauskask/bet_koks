using ChessApp.ChessLogic.Enums;
using System;

namespace ChessApp.ChessLogic
{
    public class Game
    {
        private Player[] players;
        private Board Board;
        private Player CurrentTurn;
        private GameStatus Status { get; set; }
        private List<Move> movesPlayed;

        private void Initialize(Player p1, Player p2)
        {
            players[0] = p1;
            players[1] = p2;

            Board = new Board();

            if (p1.IsWhiteSide)
            {
                CurrentTurn = p1;
            }
            else
            {
                CurrentTurn = p2;
            }

            movesPlayed = new List<Move>();
        }

        public bool isEnd()
        {
            return Status != GameStatus.ACTIVE;
        }

        private bool makeMove(Move move, Player player)
        {
            Piece sourcePiece = move.Start.Piece;
            if (sourcePiece == null)
            {
                return false;
            }

            // valid player 
            if (player != CurrentTurn)
            {
                return false;
            }

            if (sourcePiece.IsWhite != player.IsWhiteSide)
            {
                return false;
            }

            // check for valid move 

            // kill? 
            Piece destPiece = move.End.Piece;
            if (destPiece != null)
            {
                destPiece.SetKilled();
                move.PieceKilled = destPiece;
            }

            // store the move 
            movesPlayed.Add(move);

            // move piece from the stat box to end box 
            move.End.Piece = move.Start.Piece;
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
    }
}
