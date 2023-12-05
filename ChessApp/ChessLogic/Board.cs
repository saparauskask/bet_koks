using ChessApp.ChessLogic.Pieces;
using System.Text;

namespace ChessApp.ChessLogic
{
    public class Board
    {
        public Square[,] Squares;

        public Board()
        {
            Squares = new Square[8, 8];
            SetStartingPosition();
        }

        public Board GetBoardCopy()
        {
            var boardCopy = new Board();
            var squaresCopy = new Square[8, 8];
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    squaresCopy[i, j] = new Square(Squares[i, j]);
                }
            }
            boardCopy.Squares = squaresCopy;

            return boardCopy;
        }

        public void SetStartingPosition()
        {
            //white pieces
            Squares[0, 0] = new Square(0, 0, new Rook(isWhite: true));
            Squares[1, 0] = new Square(1, 0, new Knight(isWhite: true));
            Squares[2, 0] = new Square(2, 0, new Bishop(isWhite: true));
            Squares[3, 0] = new Square(3, 0, new Queen(isWhite: true));
            Squares[4, 0] = new Square(4, 0, new King(isWhite: true));
            Squares[5, 0] = new Square(5, 0, new Bishop(isWhite: true));
            Squares[6, 0] = new Square(6, 0, new Knight(isWhite: true));
            Squares[7, 0] = new Square(7, 0, new Rook(isWhite: true));

            //black pieces
            Squares[0, 7] = new Square(0, 7, new Rook(isWhite: false));
            Squares[1, 7] = new Square(1, 7, new Knight(isWhite: false));
            Squares[2, 7] = new Square(2, 7, new Bishop(isWhite: false));
            Squares[3, 7] = new Square(3, 7, new Queen(isWhite: false));
            Squares[4, 7] = new Square(4, 7, new King(isWhite: false));
            Squares[5, 7] = new Square(5, 7, new Bishop(isWhite: false));
            Squares[6, 7] = new Square(6, 7, new Knight(isWhite: false));
            Squares[7, 7] = new Square(7, 7, new Rook(isWhite: false));

            //white and black pawns
            for (int i = 0; i < 8; ++i)
            {
                Squares[i, 1] = new Square(i, 1, new Pawn(isWhite: true));
                Squares[i, 6] = new Square(i, 6, new Pawn(isWhite: false));
            }

            //empty squres
            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[j, i] = new Square(j, i, null);
                }
            }
            
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 7; i >= 0; --i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if (Squares[i, j] != null)
                    {
                        stringBuilder.Append($"{Squares[j, i]} ");
                    }
                    else
                    {
                        stringBuilder.Append(".. ");
                    }
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
