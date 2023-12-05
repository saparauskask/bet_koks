using ChessApp.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace OnlineNotes.Controllers
{
    public class ChessController : Controller
    {
        private readonly Uri baseAddress = new Uri("https://localhost:7089/api");
        private readonly HttpClient _client;

        public ChessController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string htmlBoard = "";

            using (HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/ChessApi/CreateGame"))
            {
                if (response.IsSuccessStatusCode)
                {
                    htmlBoard = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("Error! Chess board could not have been retrieved.");
                }
            }

            ViewBag.HtmlBoard = htmlBoard;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> NewGame()
        {
            string htmlBoard = "";

            using (HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/ChessApi/CreateNewGame"))
            {
                if (response.IsSuccessStatusCode)
                {
                    htmlBoard = await response.Content.ReadAsStringAsync();
                    return Ok(htmlBoard);
                }
                else
                {
                    Console.WriteLine("Error! Chess board could not have been retrieved.");
                }
            }

            return StatusCode(500);
        }


        [HttpPost]
        public async Task<IActionResult> RetrieveCoordinates(int fromX, int fromY, int toX, int toY)
        {
            Console.WriteLine($"-------Coordinates: ({fromX},{fromY}) to ({toX},{toY})");

            try
            {
                var moveData = new MoveRequest
                {
                    FromX = fromX,
                    FromY = fromY,
                    ToX = toX,
                    ToY = toY
                };

                var content = new StringContent(JsonConvert.SerializeObject(moveData), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/ChessApi/MakeMove", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string updatedHtmlBoard = await response.Content.ReadAsStringAsync();

                        return Ok(updatedHtmlBoard);
                    }
                    else
                    {
                        Console.WriteLine("Error! Move could not have been made.");
                        // Handle the error appropriately
                        return BadRequest(); // Or return an appropriate status code
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                // Handle the exception appropriately
                return StatusCode(500); // Or return an appropriate status code
            }
        }
    }
}
