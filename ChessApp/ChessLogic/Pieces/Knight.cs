namespace ChessApp.ChessLogic.Pieces
{
    public class Knight : Piece
    {
        public override char Letter => 'k';
        public Knight(bool isWhite) : base(isWhite)
        {
        }

        public Knight(Knight original) : base(original)
        {

        }
    }
}
