
using Microsoft.AspNetCore.Mvc;
using SignalrClient.Services;

namespace boardgame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignalRController : ControllerBase
    {
        private readonly ILogger<SignalRController> _logger;
        private readonly ISignalRService _signalrService;
        public SignalRController(ILogger<SignalRController> logger, ISignalRService signalrservice)
        {
            _logger = logger;
            _signalrService = signalrservice;
        }
        [HttpGet("Login")]
        public async Task<IActionResult> Login(string id)
        {
            _signalrService.Login(id);
            return Ok();
        }

        [HttpGet("SelectBoard")]
        public async Task<IActionResult> Selectboard(int id)

        {
            _signalrService.SelectBoard(id);
            return Ok();
        }


        [HttpGet("TryMoveAgent")]
        public async Task<IActionResult> TryMoveAgent(string id, int x, int y)
        {
            _signalrService.TryMoveAgent(id, x.ToString(), y.ToString());
            return Ok();
        }


    }
}
