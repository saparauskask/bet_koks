namespace ChessApp.ChessLogic.Pieces
{
    public class Queen : Piece
    {
        public override char Letter => 'Q';
        public Queen(bool isWhite) : base(isWhite)
        {
        }

        public Queen(Queen original) : base(original)
        {

        }
    }
}
