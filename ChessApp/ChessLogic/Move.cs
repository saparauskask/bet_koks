namespace ChessApp.ChessLogic
{
    public class Move
    {
        private Player Player;
        public Square Start { get; private set; }
        public Square End { get; private set; }
        public Piece PieceMoved { get; set; }
        public Piece PieceKilled { get; set; }

        public Move(Player player, Square start, Square end)
        {
            Player = player;
            Start = start;
            End = end;
        }
    }
}
