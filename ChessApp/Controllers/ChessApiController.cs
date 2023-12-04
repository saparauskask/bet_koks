using ChessApp.ChessLogic;
using ChessApp.Requests;
using ChessApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ChessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChessApiController : Controller
    {
        private readonly Game _game;
        public ChessApiController()
        {
            var p1 = new Player(true, true);
            var p2 = new Player(false, true);
            _game = new Game(p1, p2);
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
