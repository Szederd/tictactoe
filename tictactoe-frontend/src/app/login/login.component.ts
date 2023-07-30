import { Component } from '@angular/core';
import { LoginService } from './login.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  userName: string = "";
  error: string = '';
  
  // Ellenörzi hogy nincs e bejelentkezve, ha igen, akkor a lobby-ba irányítja
  constructor(private loginService: LoginService, private router: Router){
    if(this.loginService.getIsLoggedIn())
      this.router.navigate(['/lobby']);
  }

  // Bejelentkezési adatok továbbítása a Service-nek
  async onSubmit(): Promise<void> {
    const error = await this.loginService.login(this.userName);
    if (error) {
      this.error = error;
    }
  }
}
