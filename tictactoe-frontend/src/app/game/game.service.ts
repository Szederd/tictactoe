import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { BoardCell, Game } from './board';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private connection: signalR.HubConnection | undefined;

  constructor(private httpClient: HttpClient) {}

  startConnection(gameId: string): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.SIGNALR_URL) 
      .build();

      this.connection.start().then(() => {
        this.joinGameGroup(gameId);
      }).catch((err) => {
        console.error('Hiba a SignalR kapcsolattal:', err);
      });
    }
  
    // Connect to the SignalR group
    private joinGameGroup(gameId: string): void {
      if (this.connection?.state === signalR.HubConnectionState.Connected) {
        this.connection?.invoke('JoinGameGroup', gameId);
      } else {
        this.connection?.onclose(() => {
          this.connection?.start().then(() => {
            this.joinGameGroup(gameId);
          });
        });
      }
    }
  
    // Handle updates
    onMoveMade(callback: (moveData: Game) => void): void {
      this.connection?.on('MoveMade', callback);
    }

    // Get the game
    getTable(gameId: string): Observable<Game> {
      return this.httpClient.get<Game>(`${environment.BASE_URL}get-match/${gameId}`);
    }

    // Call the api with the move request
    makeMove(cell: BoardCell, gameId: string, playerId: string): Observable<any>{
      return this.httpClient.post<any>(`${environment.BASE_URL}make-move`, {
        gameId: gameId,
        nextPlayerId: playerId,
        move: cell.coord
      });
    }

    // Handle surrending
    async surrend(userId: string, gameId: string): Promise<void>{
       await this.httpClient.post(`${environment.BASE_URL}surrend`,{userId: userId, gameId: gameId}).subscribe();
    }
}
