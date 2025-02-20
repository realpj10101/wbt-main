import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CreateTeam } from '../models/create.team.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { ShowTeam } from '../models/show-team.model';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  http = inject(HttpClient)
  platformId = inject(PLATFORM_ID);
  
  private readonly _apiUrl = environment.apiUrl + 'api/team/'

  create(userInput: CreateTeam): Observable<ShowTeam | null> {
    return this.http.post<ShowTeam>(this._apiUrl + 'create', userInput);
  }
}
