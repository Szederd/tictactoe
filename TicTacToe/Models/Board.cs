using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TicTacToe.Models
{
    public class Board
    {
        public string GroupName { get; set; }
        public List<Cell> BoardCells => GetNonEmptyCells();
        public Player Player1 { get; init; } = default!;
        public Player Player2 { get; init; } = default!;
        public Player? Winner { get; set; }
        //---------- PRIVATE ----------//
        private Guid _boardId { get; init; } = default!;
        private Cell[,] _cells { get; init; }
        private Player _nextPlayer { get; set; }
        private bool _isGameOver { get; set; } = false;
        private bool _isDraw { get; set; } = false;
        private int _availableCells { get; set; } = 30 * 30;

        // Create the game board and populate the _cells with empty cells
        public Board(Player player1, Player player2)
        {
            _boardId = Guid.NewGuid();
            Player1 = player1;
            Player2 = player2;
            _nextPlayer = player1;
            GroupName = $"Game_{_boardId}";

            _cells = new Cell[30, 30];
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                        _cells[i, j] = new Cell(i, j);
                    
                }
            }
        }

        /// <summary>
        /// Check the given move if is valid and if it is then update the game's status
        /// </summary>
        /// <param name="currentPlayerId">The User's id who make the move</param>
        /// <param name="move">The move</param>
        public void MakeMove(Guid currentPlayerId, Coord move)
        {
            var currentPlayer = Player1.User.Id == currentPlayerId ? Player1 : Player2;

            if (_isGameOver) return;

            if (currentPlayer != _nextPlayer) return;

            if (move.X < 0 || move.X >= 30) return;

            if (move.Y < 0 || move.Y >= 30) return;

            if (_cells[move.Y, move.X].Mark != Mark.Empty) return;

            _cells[move.Y, move.X].Mark = currentPlayer.Mark;
            _availableCells--;

            if (CheckForWin(move, currentPlayer.Mark))
            {
                _isGameOver = true;
                Winner = currentPlayer;
            }
            else if (_availableCells == 0)
            {
                _isGameOver = true;
                _isDraw = true;
            }
            else
            {
                _nextPlayer = Player1 == currentPlayer ? Player2 : Player1;
            }
        }

        // Handle the surrender in this case the opponent is the winner
        // If the game is already over, it's return
        public void Surrend(Guid userId)
        {
            if (_isGameOver) return;

            _isGameOver = true;
            Winner = Player1.User.Id == userId ? Player2 : Player1;
        }

        // Returns the cells that have mark
        private List<Cell> GetNonEmptyCells()
        {
            var nonEmptyCells = new List<Cell>();

            foreach (var cell in _cells)
            {
                if(cell.Mark != Mark.Empty)
                    nonEmptyCells.Add(cell);
            }

            return nonEmptyCells;
        }

        // Check if 5 same marks in a row
        private bool CheckForWin(Coord move, Mark mark)
        {
            int targetCount = 5;

            int horizontalCount = CountConsecutiveMarks(move, mark, 1, 0) + CountConsecutiveMarks(move, mark, -1, 0) + 1;
            if (horizontalCount >= targetCount) return true;

            int verticalCount = CountConsecutiveMarks(move, mark, 0, 1) + CountConsecutiveMarks(move, mark, 0, -1) + 1;
            if (verticalCount >= targetCount) return true;

            int diagonal1Count = CountConsecutiveMarks(move, mark, 1, 1) + CountConsecutiveMarks(move, mark, -1, -1) + 1;
            if (diagonal1Count >= targetCount) return true;

            int diagonal2Count = CountConsecutiveMarks(move, mark, 1, -1) + CountConsecutiveMarks(move, mark, -1, 1) + 1;
            if (diagonal2Count >= targetCount) return true;

            return false;
        }

        // Count every same marks in the given direction
        private int CountConsecutiveMarks(Coord move, Mark mark, int deltaX, int deltaY)
        {
            int x = move.X + deltaX;
            int y = move.Y + deltaY;
            int count = 0;

            while (x >= 0 && x < 30 && y >= 0 && y < 30 && _cells[y, x].Mark == mark)
            {
                count++;
                x += deltaX;
                y += deltaY;
            }

            return count;
        }

        // ----- Getters -----
        public bool IsDraw() => _isDraw;

        public bool IsGameOver() => _isGameOver;

        public User? GetWinner() => Winner?.User;
        public Guid GetGameId() => _boardId;

        public Guid GetNextPlayerId() =>_nextPlayer.User.Id;

        // Get is the user is member of the game
        public bool MemeberOfTheGame(Guid userId)
        {
            return Player1.User.Id == userId || Player2.User.Id == userId;
        }

    }

    public class Cell
    {
        public Mark Mark { get; set; } = Mark.Empty;
        public Coord Coord { get; init; } = default;

        public Cell(int x, int y)
        {
            Coord = new Coord { X = x, Y = y };
        }

    }

    public enum Mark
    {
        X,
        O,
        Empty
    }
    public class Player
    {
        public User User { get; set; }
        public Mark Mark { get; set; }
    }

    public struct Coord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
