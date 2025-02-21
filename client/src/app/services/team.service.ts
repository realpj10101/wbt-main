import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CreateTeam } from '../models/create.team.model';
import { map, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { ShowTeam } from '../models/show-team.model';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  http = inject(HttpClient)
  platformId = inject(PLATFORM_ID);
  router = inject(Router);

  private readonly _apiUrl = environment.apiUrl + 'api/team/'

  create(userInput: CreateTeam): Observable<ShowTeam> {
    return this.http.post<ShowTeam>(this._apiUrl + 'create', userInput).pipe(
      map (res =>{
        this.navigateToReturnUrl();

        return res;
      })
    );
  }

  private navigateToReturnUrl(): void {
    if (isPlatformBrowser(this.platformId)) {
      const returnUrl = localStorage.getItem('returnUrl');
      if (returnUrl)
        this.router.navigate([returnUrl]);
      else 
        this.router.navigate(['teams']);
    }
  }
}
