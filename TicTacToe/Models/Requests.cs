namespace TicTacToe.Models
{
    public class NewUser
    {
        public string Name { get; set; }
    }

    public class MoveData
    {
        public Guid GameId { get; set; }
        public Guid NextPlayerId { get; init; } = default!;
        public Coord Move { get; set; }
    }

    public class SurrenderRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set;}
    }
}
