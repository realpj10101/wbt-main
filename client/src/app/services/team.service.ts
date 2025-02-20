import { inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CreateTeam } from '../models/create.team.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  http = inject(HttpClient)
  platformId = inject(PLATFORM_ID);
  
  private readonly _apiUrl = environment.apiUrl + 'api/team/'

  create(userInput: CreateTeam): Observable<CreateTeam | null> {
    return this.http.post<CreateTeam>()
  }
}
