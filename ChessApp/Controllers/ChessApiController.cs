using ChessApp.ChessLogic;
using ChessApp.Data;
using ChessApp.Requests;
using ChessApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace ChessApp.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]/[action]")]
    public class ChessApiController : Controller
    {
        private Game _game;
        private readonly ChessGamesRepository _repository;
        public ChessApiController()
        {
            _repository = ChessGamesRepository.Instance;
            _game = _repository.GetLastGame();
            if (_game == null)
            {
                var p1 = new Player(true, true);
                var p2 = new Player(false, true);
                _game = _repository.StartNewGame(p1, p2);
            }
        }

        [HttpGet(Name = "GetBoard")]
        public IActionResult GetBoard()
        {
            return Json(_game.Board.GetBoardCopy().ToString());
        }

        [HttpGet(Name = "CreateGame")]
        public IActionResult CreateGame()
        {
            var generator = new ChessBoardViewModelGenerator();
            var result = generator.GenerateHtml(_game.Board);

            return Ok(result);
        }

        [HttpGet(Name = "NewGame")]
        public IActionResult CreateNewGame()
        {
            var p1 = new Player(true, true);
            var p2 = new Player(false, true);
            var newGame = _repository.StartNewGame(p1, p2);

            var generator = new ChessBoardViewModelGenerator();
            var htmlBoard = generator.GenerateHtml(_game.Board);

            return Ok(htmlBoard);
        }

        [HttpPost(Name = "MakeMove")]
        public IActionResult MakeMove([FromBody] MoveRequest moveRequest)
        {
            if (moveRequest == null)
            {
                return BadRequest("Invalid move request");
            }

            var fromX = moveRequest.FromX;
            var fromY = moveRequest.FromY;
            var toX = moveRequest.ToX;
            var toY = moveRequest.ToY;

            var success = _game.MakeMoveWithCoordinates(_game.CurrentTurn, fromX, fromY, toX, toY);

            if (success)
            {
                var generator = new ChessBoardViewModelGenerator();
                var updatedHtmlBoard = generator.GenerateHtml(_game.Board);

                // Return the updated HTML board in the response
                return Ok(updatedHtmlBoard);
            }
            else
            {
                return BadRequest("Invalid move");
            }
        }
    }
}
