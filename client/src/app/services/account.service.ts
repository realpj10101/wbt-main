import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { Observable, map, take } from 'rxjs';
import { RegisterPlayer } from '../models/register-player.model';
import { LoginPlayer } from '../models/login-player.model';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { LoggedInUser } from '../models/logged-in-player.model';
import { ApiResponse } from '../models/helpers/apiResponse.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  //#region injects and variables
  http = inject(HttpClient);
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
  loggedInUserSig = signal<LoggedInUser | null>(null);

  private readonly baseApiUrl = environment.apiUrl + 'api/account/';

  registerPlayer(playerInput: RegisterPlayer): Observable<LoggedInUser | null> {
    return this.http.post<LoggedInUser>(this.baseApiUrl + 'register', playerInput).pipe(
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

  loginPlayer(playerInput: LoginPlayer): Observable<LoggedInUser | null> {
    return this.http.post<LoggedInUser>(this.baseApiUrl + 'login', playerInput).pipe(
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

  setCurrentPlayer(loggedInUser: LoggedInUser): void {
    this.setLoggedInPlayerRoles(loggedInUser);

    this.loggedInUserSig.set(loggedInUser);

    if (isPlatformBrowser(this.platformId)) // we make sure this code is ran on the browser and NOT server
      localStorage.setItem('loggedInUser', JSON.stringify(loggedInUser));
  }

  setLoggedInPlayerRoles(loggedInUser: LoggedInUser): void {
    loggedInUser.roles = []; // We have to initialize it before pushing. Otherwise, it's 'undefined' and push will not work.

    const roles: string | string[] = JSON.parse(atob(loggedInUser.token.split('.')[1])).role; // get the token's 2nd part then select role

    Array.isArray(roles) ? loggedInUser.roles = roles : loggedInUser.roles.push(roles);
  }

  logOut(): void {
    this.loggedInUserSig.set(null);

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

  loadLoggedInUserFromStorage(): void {
    if (isPlatformBrowser(this.platformId)) {
      const userJson = localStorage.getItem('loggedInUser');
      if (userJson) {
        const user = JSON.parse(userJson) as LoggedInUser;
        this.setLoggedInPlayerRoles(user);
        this.loggedInUserSig.set(user);
      }
    }
  }
}
