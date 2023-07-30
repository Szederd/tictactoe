import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, NgModel } from '@angular/forms';

import { AppComponent } from './app.component';
import { GameComponent } from './game/game.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';
import { LobbyComponent } from './lobby/lobby.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Defining routes
const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'game/:boardId', component: GameComponent, canActivate: [AuthGuard]},
  {path: 'lobby', component: LobbyComponent, canActivate: [AuthGuard]},
  {path: '', redirectTo: '/lobby', pathMatch: 'full'}
];

@NgModule({
  declarations: [
    AppComponent,
    GameComponent,
    LobbyComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
