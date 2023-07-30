using Microsoft.AspNetCore.SignalR;
using TicTacToe.Models;
using TicTacToe.Services;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService) => _gameService = gameService;

        // Send message to clients in the group
        public async Task MakeMove(MoveData moveData)
        {

            var game = _gameService.MoveMade(moveData);
            if (game != null)
            {
                await Clients.Group(game.GroupName).SendAsync("MoveMade", new GameResponse
                {
                    Board = game,
                    GameStatus = game.IsGameOver() ? GameStatus.Over : GameStatus.Ongoing,
                    NextPlayer = game.GetNextPlayerId()
                });
            }
        }

        // Add client to group
        public async Task JoinGameGroup(Guid gameId)
        {
            var groupName = $"Game_{gameId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

    }
}
