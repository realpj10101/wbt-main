import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { environment } from '../../environments/environment.development';
import { RegisterPlayer } from '../models/register-player.model';
import { map, Observable, take } from 'rxjs';
import { LoginPlayer } from '../models/login-player.model';
import { ApiResponse } from '../models/helpers/apiResponse.model';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class CoachAccountService {
  http = inject(HttpClient);
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
  loggedInCoachSig = signal<LoggedInPlayer | null>(null);

  private readonly _apiUrl = environment.apiUrl + 'api/coachaccount/';

  registerCoach(userInput: RegisterPlayer): Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this._apiUrl + 'register', userInput).pipe(
      map(res => {
        if (res) {
          this.setCurrentCoach(res);

          this.navigateToReturnUrl(); // navigate to url which player tried before log-in. else: default

          return res;
        }

        return null;
      })
    );
  }

  loginCoach(userInput: LoginPlayer): Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this._apiUrl + 'login', userInput).pipe(
      map(res => {
        if (res) {
          this.setCurrentCoach(res);

          this.navigateToReturnUrl();

          return res;
        }

        return null;
      })
    )
  }

  /**
   * Check if player's token is still valid or log them out
   * Called in app.component.ts
   * @returns Observable<LoggedInCoach | null>
   */
  authorizeLoggedInCoach(): void {
    this.http.get<ApiResponse>(this._apiUrl)
      .pipe(
        take(1))
      .subscribe({
        next: res => {
          if (res.message)
            console.log(res.message);
        },
        error: err => {
          console.log(err.error);

        }
      })
  }

  setCurrentCoach(loggedInUser: LoggedInPlayer): void {
    this.setLoggedInCoachRoles(loggedInUser);

    this.loggedInCoachSig.set(loggedInUser);

    if (isPlatformBrowser(this.platformId)) // we make sure this code ran on browser not on server
      localStorage.setItem('loggedInCoach', JSON.stringify(loggedInUser));
  }

  setLoggedInCoachRoles(loggedInUser: LoggedInPlayer): void {
    loggedInUser.roles = []; // We have to initialize it before pushing. Otherwise, its 'undefind' and push will not work

    const roles: string | string[] = JSON.parse(atob(loggedInUser.token.split('.')[1])).role; // get the tokens 2nd part then select role
  }

  logOut(): void {
    this.loggedInCoachSig.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear(); // delete all browser's localStorage's items at once
    }

    this.router.navigateByUrl('coach-account/login');
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
