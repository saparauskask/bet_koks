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
            Squares[0, 1] = new Square(0, 1, new Knight(isWhite: true));
            Squares[0, 2] = new Square(0, 2, new Bishop(isWhite: true));
            Squares[0, 3] = new Square(0, 3, new Queen(isWhite: true));
            Squares[0, 4] = new Square(0, 4, new King(isWhite: true));
            Squares[0, 5] = new Square(0, 5, new Bishop(isWhite: true));
            Squares[0, 6] = new Square(0, 6, new Knight(isWhite: true));
            Squares[0, 7] = new Square(0, 7, new Rook(isWhite: true));

            //black pieces
            Squares[7, 0] = new Square(0, 0, new Rook(isWhite: false));
            Squares[7, 1] = new Square(0, 1, new Knight(isWhite: false));
            Squares[7, 2] = new Square(0, 2, new Bishop(isWhite: false));
            Squares[7, 3] = new Square(0, 3, new Queen(isWhite: false));
            Squares[7, 4] = new Square(0, 4, new King(isWhite: false));
            Squares[7, 5] = new Square(0, 5, new Bishop(isWhite: false));
            Squares[7, 6] = new Square(0, 6, new Knight(isWhite: false));
            Squares[7, 7] = new Square(0, 7, new Rook(isWhite: false));

            //white and black pawns
            for (int i = 0; i < 8; ++i)
            {
                Squares[1, i] = new Square(0, i, new Pawn(isWhite: true));
                Squares[6, i] = new Square(6, i, new Pawn(isWhite: false));
            }

            /*
            for (int i = 2; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares[i][j] = new Square(i, j, null);
                }
            }
            */
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if (Squares[i, j] != null)
                    {
                        stringBuilder.Append($"{Squares[i, j]} ");
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
