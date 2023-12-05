namespace ChessApp.ChessLogic
{
    public class Move
    {
        public Square Start { get; private set; }
        public Square End { get; private set; }
        public Piece PieceMoved { get; set; }
        public Piece PieceKilled { get; set; }

        public Move(Square start, Square end)
        {
            Start = start;
            End = end;
        }
    }
}
