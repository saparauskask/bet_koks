namespace ChessApp.ChessLogic.Pieces
{
    public class King : Piece
    {
        public override char Letter => 'K';
        public King(bool isWhite) : base(isWhite)
        {
        }

        public King(King original) : base(original)
        {

        }
    }
}
