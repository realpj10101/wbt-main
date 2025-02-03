import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { Observable, map, take } from 'rxjs';
import { RegisterPlayer } from '../models/register-player.model';
import { LoginPlayer } from '../models/login-player.model';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { ApiResponse } from '../models/helpers/apiResponse.model';

@Injectable({
  providedIn: 'root'
})
export class RegisterPlayerService {
  //#region injects and variables
  http = inject(HttpClient);
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
  loggedInPlayerSig = signal<LoggedInPlayer | null>(null);

  private readonly baseApiUrl = environment.apiUrl + 'account/';

  registerPlayer(playerInput: RegisterPlayer): Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this.baseApiUrl + 'register', playerInput).pipe(
      map(playerResponse => {
        if (playerResponse) {
          this.setCurrentPlayer(playerResponse);

          this.navigateToReturnUrl(); // navigate to url wich player tried before log-in. else: default

          return playerResponse;
        }

        return null;
      })
    );
  }

  loginPlayer(playerInput: LoginPlayer): Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this.baseApiUrl + 'login', playerInput).pipe(
      map(playerResponse => {
        if (playerResponse) {
          this.setCurrentPlayer(playerResponse);

          this.navigateToReturnUrl();

          return playerResponse;
        }

        return null;
      })
    );
  }

  /**
   * Check if player's token is still valid or log them out.
   * Called in app.component.ts
   * @returns Observable<LoggedInPlayer | null>
   */
  authorizeLoggedInPlayer(): void {
    this.http.get<ApiResponse>(this.baseApiUrl)
      .pipe(
        take(1))
      .subscribe({
        next: res => {
          if (res.message)
            console.log(res.message);
        }, 
        error: err => {
          console.log(err.error);
          this.logOut()
        }
      });
  }

  setCurrentPlayer(loggedInPlayer: LoggedInPlayer): void {
    this.setLoggedInPlayerRoles(loggedInPlayer);

    this.loggedInPlayerSig.set(loggedInPlayer);

    if (isPlatformBrowser(this.platformId)) // we make sure this code is ran on the browser and NOT server
      localStorage.setItem('loggedInPlayer', JSON.stringify(loggedInPlayer));
  }

  setLoggedInPlayerRoles(loggedInPlayer: LoggedInPlayer): void {
    loggedInPlayer.roles = []; // We have to initialize it before pushing. Otherwise, it's 'undefined' and push will not work.

    const roles: string | string[] = JSON.parse(atob(loggedInPlayer.token.split('.')[1])).role; // get the token's 2nd part then select role

    Array.isArray(roles) ? loggedInPlayer.roles = roles : loggedInPlayer.roles.push(roles);
  }

  logOut(): void {
    this.loggedInPlayerSig.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear(); // delete all browser's localStorage's items at once  
    }

    this.router.navigateByUrl('account/login');
  }

  private navigateToReturnUrl(): void {
    if (isPlatformBrowser(this.platformId)) {
      const returnUrl = localStorage.getItem('returnUrl');
      if (returnUrl)
        this.router.navigate([returnUrl]);
      else
        this.router.navigate(['members']);

      if (isPlatformBrowser(this.platformId)) // we make sure this code is ran on the browser and NOT server
        localStorage.removeItem('returnUrl');
    }
  }
}
