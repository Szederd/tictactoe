import { Component, OnDestroy } from '@angular/core';
import { LoginService } from '../login/login.service';
import { LobbyService } from './lobby.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent implements OnDestroy {
  isSearching: boolean = false;
  userName: string | null = null;
  ngUnsubscribe = new Subject();


  constructor(private loginService: LoginService, private lobbyService: LobbyService, private router: Router) { 
    this.userName = this.loginService.getUserName();
  }

  /*
  * Searching for opponent.
  * The request can be canceled.
  * If the response is an error then the client logout the user.
  */
  searchMatch(): void{
    const userId = this.loginService.getUserId();

    if (userId != null) {
      this.isSearching = true;
      this.lobbyService.findMatch(userId)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(success => {
        this.isSearching = false;
        this.router.navigate([`/game/${success}`]);
      },
      error => {
        this.isSearching = false;
        this.loginService.logout();
      });
    }
  }

  // Cancel the request
  cancelSearch(): void{
    this.isSearching = false;
    this.ngUnsubscribe.next(true);
  }

  // Logingout
  logout(): void{
    if(confirm("Biztos ki szeretnél lépni?"))
      this.loginService.logout();
  }

  // If page close
  ngOnDestroy(): void{
    this.ngUnsubscribe.next(true);
    this.ngUnsubscribe.complete();
  }
}
