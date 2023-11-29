using ChessApp.ChessLogic;
using Microsoft.AspNetCore.Mvc;

namespace ChessApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}
