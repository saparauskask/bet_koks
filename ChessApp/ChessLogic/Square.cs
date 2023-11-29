using ChessApp.ChessLogic.Pieces;
using System.Net.NetworkInformation;

namespace ChessApp.ChessLogic
{
    public class Square
    {
        public Piece Piece { set; get;}
        public int X { get; set; }
        public int Y { get; set; }

        public Square(int x, int y, Piece piece)
        {
            Piece = piece;
            X = x;
            Y = y;
        }

        public Square(Square original)
        {
            if (original != null && original.Piece != null)
            {
                // Use the copy constructor of the specific piece class
                if (original.Piece is Knight knight)
                {
                    Piece = new Knight(knight);
                }
                else if (original.Piece is Bishop bishop)
                {
                    Piece = new Bishop(bishop);
                }
                // Add other piece types as needed
                else if (original.Piece is Pawn pawn)
                {
                    Piece = new Pawn(pawn);
                }
                else if (original.Piece is Rook rook)
                {
                    Piece = new Rook(rook);
                }
                else if (original.Piece is Queen queen)
                {
                    Piece = new Queen(queen);
                }
                else if (original.Piece is King king)
                {
                    Piece = new King(king);
                }

                X = original.X;
                Y = original.Y;
            }
        }
        public override string ToString()
        {
            return $"{Piece} ";
        }
    }
}
