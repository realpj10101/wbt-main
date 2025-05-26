import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInUser } from '../models/logged-in-player.model';
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
  loggedInUserSig = signal<LoggedInUser | null>(null);
  profilePhotoUrl = signal<string | null>(null);

  private readonly _apiUrl = environment.apiUrl + 'api/coachaccount/';

  setProfilePhotoUrl(url: string) {
    this.profilePhotoUrl.set(url);
  }

  registerCoach(userInput: RegisterPlayer): Observable<LoggedInUser | null> {
    return this.http.post<LoggedInUser>(this._apiUrl + 'register', userInput).pipe(
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

  loginCoach(userInput: LoginPlayer): Observable<LoggedInUser | null> {
    return this.http.post<LoggedInUser>(this._apiUrl + 'login', userInput).pipe(
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
          this.logOut();
        }
      })
  }

  setCurrentCoach(loggedInUser: LoggedInUser): void {
    this.setLoggedInCoachRoles(loggedInUser);

    this.loggedInUserSig.set(loggedInUser);
  
    if (isPlatformBrowser(this.platformId)) // we make sure this code ran on browser not on server
      localStorage.setItem('loggedInUser', JSON.stringify(loggedInUser));
  }

  setLoggedInCoachRoles(loggedInUser: LoggedInUser): void {
    loggedInUser.roles = []; // We have to initialize it before pushing. Otherwise, its 'undefind' and push will not work

    const roles: string | string[] = JSON.parse(atob(loggedInUser.token.split('.')[1])).role; // get the tokens 2nd part then select role

    Array.isArray(roles) ? loggedInUser.roles = roles : loggedInUser.roles.push(roles);
  }

  logOut(): void {
    this.loggedInUserSig.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear(); // delete all browser's localStorage's items at once
    }

    this.router.navigateByUrl('coachaccount/login');
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
