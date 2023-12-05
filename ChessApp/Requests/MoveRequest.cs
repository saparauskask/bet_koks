﻿using ChessApp.ChessLogic;

namespace ChessApp.Requests
{
    public class MoveRequest
    {
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
    }
}
