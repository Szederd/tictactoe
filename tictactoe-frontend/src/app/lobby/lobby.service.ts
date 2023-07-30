import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LobbyService {

  constructor(private httpClient: HttpClient) { }

  // Create the FindMatch request
  findMatch(playerId: string): Observable<string>{
    return this.httpClient.get<string>(`${environment.BASE_URL}find-match/${playerId}`);
  }
}
