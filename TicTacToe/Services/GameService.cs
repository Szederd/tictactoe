using Microsoft.AspNetCore.SignalR;
using System.Threading;
using TicTacToe.Controllers;
using TicTacToe.Hubs;
using TicTacToe.Models;
using static TicTacToe.Models.Board;

namespace TicTacToe.Services
{
    public class GameService : IGameService
    {
        private readonly List<Board> _games = new List<Board>();
        private readonly List<User> _users = new List<User>();
        private readonly IHubContext<GameHub> _hubContext;
        private readonly Random _random = new Random();

        public GameService(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        public User? AddUser(NewUser user)
        {
            if (_users.Where(u => u.UserName.ToLower() == user.Name.ToLower()).Count() > 0) return null;

            User newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = user.Name,
                IsAvailable = false,
            };
            _users.Add(newUser);

            Console.WriteLine($"User {newUser.UserName} added with id: {newUser.Id}");
            return newUser;
        }

        public async Task<Guid> PairUpPlayersAsync(Guid userId, CancellationToken cT)
        {
            if(_users.FirstOrDefault(u => u.Id == userId) is null)
                return Guid.Empty;
            try
            {
                _users.FirstOrDefault(u => u.Id == userId).IsAvailable = true;

                while (!cT.IsCancellationRequested)
                {
                    var availablePlayersList = _users.Where(u => u.IsAvailable && u.Id != userId);

                    if (_games.FirstOrDefault(g => g.MemeberOfTheGame(userId) && !g.IsGameOver()) is not null) return _games.FirstOrDefault(g => g.MemeberOfTheGame(userId) && !g.IsGameOver()).GetGameId();

                    if (availablePlayersList.Count() == 0)
                    {
                        await Task.Delay(500, cT);
                        continue;
                    }

                    if (availablePlayersList.Count() == 1)
                    {
                        cT.ThrowIfCancellationRequested();
                        var randomResult = _random.Next(2);

                        var player1 = availablePlayersList.FirstOrDefault();
                        var player2 = _users.FirstOrDefault(u => u.Id == userId);

                        player1.IsAvailable = false;
                        player2.IsAvailable = false;
                        

                        var newGame = new Board(
                            new Player()
                            {
                                User = randomResult == 0 ? player1 : player2,
                                Mark = Mark.X
                            },
                            new Player()
                            {
                                User = randomResult == 0 ? player2 : player1,
                                Mark = Mark.O
                            });
                        _games.Add(newGame);
                        Console.WriteLine($"New board with id: {newGame.GetGameId} has been created");
                        _hubContext.Groups.AddToGroupAsync(userId.ToString(), newGame.GroupName);
                        _hubContext.Groups.AddToGroupAsync(player1.Id.ToString(), newGame.GroupName);

                        return newGame.GetGameId();
                    }
                }
                cT.ThrowIfCancellationRequested();
                return Guid.Empty;
            }
            finally
            {
                _users.FirstOrDefault(u => u.Id == userId).IsAvailable = false;
            }
            
        }

        public Board? GetMatch(Guid id)
        {
            return _games.FirstOrDefault(m => m.GetGameId() == id);
        }


        public Board? MoveMade(MoveData moveData)
        {
            var board = _games.FirstOrDefault(g => g.GetGameId() == moveData.GameId);

            if (board == null) return null;

            board.MakeMove(moveData.NextPlayerId, moveData.Move);

            return board;
        }

        public Board? Surrend(SurrenderRequest surrend)
        {
            var board = _games.FirstOrDefault(g => g.GetGameId() == surrend.GameId);
            if (board == null) return null;

            board.Surrend(surrend.UserId);

            return board;
        }
    }

    public interface IGameService
    {
        /// <summary>
        ///  Create and return an inmemory user
        /// </summary>
        /// <param name="user">The NewUser request</param>
        /// <returns>Returns the new user</returns>
        public User? AddUser(NewUser user);
        /// <summary>
        /// Find an opponent, create a new game, add to the SignalR group and return the game's id
        /// If any error it returns Guid.Empty
        /// It handles async because if there is no available opponent it waits some time
        /// </summary>
        /// <param name="userId">The user that looking for a match</param>
        /// <param name="cT">Pass the cancellationToken to handle if the request cancelled</param>
        /// <returns>Returns the game's id or Guid.Empty</returns>
        public Task<Guid> PairUpPlayersAsync(Guid userId, CancellationToken cT);
        /// <summary>
        /// Returns the found game
        /// </summary>
        /// <param name="id">The id of the game you are looking for</param>
        /// <returns>The found game of the given id</returns>
        public Board? GetMatch(Guid id);
        /// <summary>
        /// Handle the moveData
        /// </summary>
        /// <param name="moveData">The MoveData request</param>
        /// <returns>Returns the Board if it is a valid request</returns>
        public Board? MoveMade(MoveData moveData);
        /// <summary>
        /// Handle the surrender request
        /// </summary>
        /// <param name="surrend"></param>
        /// <returns>Returns the board if the request is valid</returns>
        public Board? Surrend(SurrenderRequest surrend);
    }
}
