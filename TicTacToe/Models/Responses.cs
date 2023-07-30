namespace TicTacToe.Models
{
    public class GameResponse
    {
        public GameStatus GameStatus { get; init; } = default!;
        public Board Board { get; init; } = default!;
        public Guid NextPlayer { get; init; } = default!;
    }

    public enum GameStatus
    {
        Ongoing,
        Over
    }
}
