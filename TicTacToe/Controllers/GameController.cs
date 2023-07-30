using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using TicTacToe.Hubs;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Controllers
{
    [Route("/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IHubContext<GameHub> _hubContext;

        public GameController(IGameService gameService, IHubContext<GameHub> hubContext)
        {
            _gameService = gameService;
            _hubContext = hubContext;
        }

        // Register new Player
        [HttpPost("register")]
        public IActionResult RegisterPlayer([FromBody] NewUser user)
        {
            User? response = _gameService.AddUser(user);
            if (response is null) return BadRequest("A felhasználónév foglalt.");
            return Ok(response);
        }

        // Find new match for the given User, use cT to handle cancelation
        [HttpGet("find-match/{playerId}")]
        public async Task<IActionResult> NewGame(Guid playerId, CancellationToken cT)
        {
            try
            {
                var result = await _gameService.PairUpPlayersAsync(playerId, cT);

                if (result == Guid.Empty) return BadRequest();

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499);
            }
        }

        // Get the match's data
        [HttpGet("get-match/{id}")]
        public IActionResult GetMatch(Guid id)
        {
            var result = _gameService.GetMatch(id);

            if(result is null) return BadRequest();

            return Ok(new GameResponse
            {
                Board = result,
                GameStatus = result.IsGameOver() ? GameStatus.Over : GameStatus.Ongoing,
                NextPlayer = result.GetNextPlayerId()
            });
        }

        // Post a move
        [HttpPost("make-move")]
        public IActionResult MakeMove([FromBody] MoveData moveData)
        {
            var game = _gameService.MoveMade(moveData);

            if(game is null) return BadRequest();

            var response = new GameResponse
            {
                Board = game,
                GameStatus = game.IsGameOver() ? GameStatus.Over : GameStatus.Ongoing,
                NextPlayer = game.GetNextPlayerId()
            };

            // Invoke SignalR group
            _hubContext.Clients.Group($"Game_{moveData.GameId}").SendAsync("MoveMade", response);
            return Ok(response);
        }

        // Handle surrend
        [HttpPost("surrend")]
        public IActionResult Surrend([FromBody] SurrenderRequest surrend)
        {
            var game = _gameService.Surrend(surrend);

            if (game is null) return BadRequest();

            var response = new GameResponse
            {
                Board = game,
                GameStatus = game.IsGameOver() ? GameStatus.Over : GameStatus.Ongoing,
                NextPlayer = game.GetNextPlayerId()
            };
            // Invoke SignalR group
            _hubContext.Clients.Group($"Game_{surrend.GameId}").SendAsync("MoveMade", response);
            return Ok(response);
        }



    }
}
