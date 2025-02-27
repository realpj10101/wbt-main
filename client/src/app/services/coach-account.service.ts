import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { environment } from '../../environments/environment.development';
import { RegisterPlayer } from '../models/register-player.model';
import { Observable, take } from 'rxjs';
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

  registerCoach(userInput: RegisterPlayer):Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this._apiUrl + 'register', userInput);
  }

  loginCoach(userInput: LoginPlayer): Observable<LoggedInPlayer | null> {
    return this.http.post<LoggedInPlayer>(this._apiUrl + 'login', userInput);
  }

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

  setLoggedInCoachRoles(loggedInUser: LoggedInPlayer): void {
    loggedInUser.roles = []; // We have to initialize

    const roles: string | string[] = JSON.parse(atob(loggedInUser.token.split('.')[1])).role;
  }

  logOut(): void {
    this.loggedInCoachSig.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear();
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
