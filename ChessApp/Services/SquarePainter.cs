namespace ChessApp.Services
{
    public class SquarePainter
    {
        public bool IsWhite { get; set; }
        public string currentColor;
        private string WPawn = "\u2659";
        private string WRook = "\u2656";
        private string WKnight = "\u2658";
        private string WBishop = "\u2657";
        private string WQueen = "\u2655";
        private string WKing = "\u2654";
        
        private string BPawn = "\u265f";
        private string BRook = "\u265c";
        private string BKnight = "\u265e";
        private string BBishop = "\u265d";
        private string BQueen = "\u265b";
        private string BKing = "\u265a";

        private const string dark = "\"dark\"";
        private const string light = "\"light\"";

        public SquarePainter(bool startingColor)
        {
            IsWhite = startingColor;
            currentColor = IsWhite ? light : dark;
        }

        public void switchColor()
        {
            if (IsWhite)
            {
                currentColor = dark;
                IsWhite = !IsWhite;
            }
            else
            {
                currentColor = light;
                IsWhite = !IsWhite;
            }
        }

        public string GetPawn(bool color)
        {
            return color ? WPawn : BPawn;
        }
        public string GetRook(bool color)
        {
            return color ? WRook : BRook;
        }
        public string GetKnight(bool color)
        {
            return color ? WKnight : BKnight;
        }
        public string GetBishop(bool color)
        {
            return color ? WBishop : BBishop;
        }
        public string GetKing(bool color)
        {
            return color ? WKing : BKing;
        }
        public string GetQueen(bool color)
        {
            return color ? WQueen: BQueen;
        }
    }          
}
