using ChessApp.ChessLogic;
using ChessApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChessApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChessController : Controller
    {
        public ChessController()
        {
        }

        [HttpGet(Name = "GetBoard")]
        public IActionResult GetBoard()
        {
            var board = new Board();
            return Json(board.GetBoardCopy().ToString());
        }

        [HttpGet(Name = "CreateGame")]
        public IActionResult CreateGame()
        {
            var p1 = new Player(true, true);
            var p2 = new Player(false, true);
            Game game = new Game(p1, p2);

            var generator = new ChessBoardViewModelGenerator();
            var result = generator.GenerateHtml(game.Board);

            return Ok(result);
        }
    }
}
