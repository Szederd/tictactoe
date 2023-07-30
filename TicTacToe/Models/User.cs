namespace TicTacToe.Models
{
    public class User
    {
        public Guid Id { get; init; } = default!;
        public string UserName { get; init; } = default!;
        public bool IsAvailable { get; set; }
    }
}
