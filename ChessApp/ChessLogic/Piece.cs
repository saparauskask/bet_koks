namespace ChessApp.ChessLogic
{
    public abstract class Piece
    {
        public bool IsKilled { get; set; }
        public bool IsWhite { get; set; }

        public abstract char Letter { get; }

        protected Piece(bool isWhite)
        {
            IsKilled = false;
            IsWhite = isWhite;
        }

        protected Piece(Piece original)
        {
            if (original != null)
            {
                IsKilled = original.IsKilled;
                IsWhite = original.IsWhite;
            }
        }

        public override string ToString()
        {
            if (IsKilled)
            {
                return " ";
            }

            return Letter.ToString();
        }

        public void SetKilled()
        { IsKilled = true; }
    }
}
