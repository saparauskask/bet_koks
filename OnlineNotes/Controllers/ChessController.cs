using Microsoft.AspNetCore.Mvc;

namespace OnlineNotes.Controllers
{
    public class ChessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
