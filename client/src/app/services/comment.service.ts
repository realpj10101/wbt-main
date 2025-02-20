import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { environment } from '../../environments/environment.development';
import { CommentInput } from '../models/comment.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model';
import { UserComment } from '../models/user-comment.model';
import { CommentParams } from '../models/helpers/comment-params.model';
import { PaginatedResult } from '../models/helpers/pagination-result.model';
import { Member } from '../models/member.model';
import { __param } from 'tslib';
import { PaginationHandler } from '../extension/paginationHandler';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly _http = inject(HttpClient);
  private paginationHandler = new PaginationHandler();
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
  loggedInPlayerSig = signal<LoggedInPlayer | null>(null);

  private readonly _apiUrl = environment.apiUrl + 'api/comment/';

  add(targetMemberUserName: string | undefined, content: CommentInput): Observable<ApiResponse> {
    console.log('com serv')
    return this._http.post<ApiResponse>(this._apiUrl + 'add/' + targetMemberUserName, content);
  }

  getAll(commentParams: CommentParams): Observable<PaginatedResult<Member[]>> {
    let params = new HttpParams();

    if (commentParams) {
      params = params.append('pageNumber', commentParams.pageNumber);
      params = params.append('pageSize', commentParams.pageSize);
      params = params.append('predicate', commentParams.predicate);
    }

    return this.paginationHandler.getPaginatedResult<Member[]>(this._apiUrl, params);
  }

  gerAllUserComments(targetUserName: string | undefined | null): Observable<UserComment[]> {
    console.log('service', targetUserName);
    return this._http.get<UserComment[]>(this._apiUrl + 'get-user-comments/' + targetUserName);
  }

  remove(targetUserName: string | null): Observable<ApiResponse> {
    return this._http.delete<ApiResponse>(this._apiUrl + 'remove/' + targetUserName);
  }
}
