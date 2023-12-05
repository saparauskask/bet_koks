using System;

namespace ChessApp.ChessLogic
{
    public class Player
    {
        public bool IsWhiteSide { get; private set; }
        public bool IsHumanPlayer { get; private set; }

        public Player(bool isWhiteSide, bool isHumanPlayer)
        {
            IsHumanPlayer = isHumanPlayer;
            IsWhiteSide = isWhiteSide;
        }
    }
}
