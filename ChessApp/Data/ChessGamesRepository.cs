using ChessApp.ChessLogic;

namespace ChessApp.Data
{

    public class ChessGamesRepository
    {
        private static ChessGamesRepository _instance;
        private static readonly object LockObject = new object();

        private readonly List<Game> games;

        private ChessGamesRepository()
        {
            games = new List<Game>();
        }

        public static ChessGamesRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new ChessGamesRepository();
                        }
                    }
                }

                return _instance;
            }
        }

        public Game StartNewGame(Player player1, Player player2)
        {
            Game newGame = new Game(player1, player2);
            games.Add(newGame);

            return newGame;
        }

        public Game GetLastGame()
        {
            if (games.Count > 0)
            {
                return games[games.Count - 1];
            }

            return null;
        }
    }
}

