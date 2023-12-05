namespace ChessApp.ChessLogic.Pieces
{
    public class Bishop : Piece
    {
        public override char Letter => 'b';

        public Bishop(bool isWhite) : base(isWhite)
        {
        }

        public Bishop(Bishop original) : base(original)
        {

        }
    }
}
