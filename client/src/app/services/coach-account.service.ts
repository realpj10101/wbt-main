import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { environment } from '../../environments/environment.development';
import { RegisterPlayer } from '../models/register-player.model';
import { Observable } from 'rxjs';
import { LoginPlayer } from '../models/login-player.model';

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
}
