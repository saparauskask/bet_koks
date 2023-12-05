namespace ChessApp.ChessLogic.Pieces
{
    public class Pawn : Piece
    {
        public override char Letter => 'p';
        public Pawn(bool isWhite) : base(isWhite)
        {
        }

        public Pawn(Pawn original) : base(original)
        {

        }
    }
}
