import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { GameService } from './game.service';
import { LoginService } from '../login/login.service';
import { Game, Mark, BoardCell, Player, GameStatus } from './board';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  board: BoardCell[][] = [];
  game: Game | null = null;
  currentPlayer: 'X' | 'O' = 'X';
  gameId: string = '';
  userId: string | null = '';
  player: Player | null = null;

  constructor(private gameService: GameService, private route: ActivatedRoute, private loginService: LoginService, private router: Router) {  }

  /*
  * Get the game's and user's id. If it is not valid, then navigatet to the lobby
  * Create SignalR connection.
  * Get the game and create the board
  */ 
  ngOnInit(): void {
    this.userId = this.loginService.getUserId();

    this.route.params.subscribe(params => {
      this.gameId = params['boardId'];
    });

    if(!this.userId || !this.gameId)
      this.router.navigate(['/lobby']);
    
    this.gameService.startConnection(this.gameId);

    this.gameService.onMoveMade((moveData: Game) => {
      this.game = moveData;
      this.updateTable();
    });

    this.gameService.getTable(this.gameId).subscribe(success => {
      this.game = success;
      this.createBoard();
      this.player = success.board.player1.user.id == this.userId ? success.board.player1 : success.board.player2;
    },
    error => {
      this.router.navigate(['/lobby']);
    });
  }

  // get Enum value
  getMarkAsString(mark: Mark): string {
    return Mark[mark];
  }

  /*
  * Checking the game status and if it is 1, then navigate to the lobby otherwise update the game board
  */
  private updateTable(): void{
    if(this.game?.gameStatus == 1)
    {
      alert(`Játék vége! A nyertes: ${this.game.board.winner?.user.userName}`);
      this.router.navigate(['/lobby']);
    }
    for(let cell of this.game?.board?.boardCells!){
      this.board[cell.coord.y][cell.coord.x] = cell;
    }
  }

  // Create game board
  private createBoard(): void {
    this.board = [];
  
    for (let i = 0; i < 30; i++) {
      this.board.push([]);
      for (let j = 0; j < 30; j++) {
        this.board[i].push({
          mark: 3,
          coord: { x: i, y: j },
        });
      }
    }
    // If the page is reloaded then it needs to re update it.
    this.updateTable();
  }

  // Handle the cell click and sent to the server
  cellClicked(cell: BoardCell): void {
    if(this.game?.nextPlayer !== this.player?.user?.id)
      return;

    this.gameService.makeMove(cell, this.gameId, this.userId!).subscribe(success => {
      this.game = success;
      this.updateTable();
    });
  }

  // Surrender or leave the game. It depends on the game status.
  async leave(): Promise<void> {
    if(confirm("Biztos ki szeretnél lépni?")){
      if(this.game?.gameStatus == GameStatus.Ongoing)
        await this.gameService.surrend(this.userId!, this.gameId);
      this.router.navigate(['/lobby']);
    }
  }
}
