import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private isLoggedIn: boolean = false;
  private userId: string | null = null;

  // load user id if logged in
  constructor(private httpClient: HttpClient, private router: Router){ 
    this.userId = sessionStorage.getItem('user-id');
   }

   // Send and handle login to the server
   async login(userName: string): Promise<string> {
    try {
      const res = await this.httpClient.post<any>(`${environment.BASE_URL}register`, { name: userName }).toPromise();
      this.isLoggedIn = true;
      this.userId = res.id;
      sessionStorage.setItem('user', JSON.stringify(res));
      this.router.navigate(['/lobby']);
      return '';
    } catch (error: any) {
      return error.error;
    }
  }

  // Delete user's data and navigate to login
  logout(): void {
    this.isLoggedIn = false;
    this.userId = null;
    sessionStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  // Check if user is logged in
  getIsLoggedIn(): boolean {
    if(!this.userId){
      const user = sessionStorage.getItem('user');
      if(user)
        this.userId = JSON.parse(user)['id'];
    }
      
    this.isLoggedIn = this.userId ? true : false;
    
    return this.isLoggedIn;
  }

  // Return the user's id or null
  getUserId(): string | null {
    return this.userId;
  }

  // Return the user's name or null
  getUserName(): string | null{
    const user = this.getUser();
    return user['userName'];
  }

  // Return the user or null
  private getUser(): any | null {
    const user = sessionStorage.getItem('user');

    if(!user){
      this.router.navigate(['/login']);
      return null;
    }

    return JSON.parse(user);
  }
}
