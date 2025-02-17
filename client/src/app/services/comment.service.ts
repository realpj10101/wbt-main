import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { environment } from '../../environments/environment.development';
import { CommentInput } from '../models/comment.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model';
import { UserComment } from '../models/user-comment.model';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly _http = inject(HttpClient);
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
  loggedInPlayerSig = signal<LoggedInPlayer | null>(null);

  private readonly _apiUrl = environment.apiUrl + 'api/comment/';

  add(targetMemberUserName: string | undefined, content: CommentInput): Observable<ApiResponse> {
    console.log('com serv')
    return this._http.post<ApiResponse>(this._apiUrl + 'add/' + targetMemberUserName, content);
  }

  gerAllUserComments(targetUserName: string | undefined | null): Observable<UserComment[]> {
    console.log('service', targetUserName);
    return this._http.get<UserComment[]>(this._apiUrl + 'get-user-comments/' + targetUserName);
  }

  delete(targetUSerName)
}
