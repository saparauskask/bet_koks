namespace ChessApp.ChessLogic.Pieces
{
    public class Rook : Piece
    {
        public override char Letter => 'r';
        public Rook(bool isWhite) : base(isWhite)
        {
        }

        public Rook(Rook original) : base(original)
        {

        }
    }
}
