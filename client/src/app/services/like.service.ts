import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PaginationHandler } from '../extension/paginationHandler';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model';
import { FollowParams } from '../models/helpers/follow-params.model';
import { PaginatedResult } from '../models/helpers/pagination-result.model';
import { Member } from '../models/member.model';
import { LikeParams } from '../models/helpers/like-params.model';

@Injectable({
  providedIn: 'root'
})
export class LikeService {
  private readonly _http = inject(HttpClient);

  private paginationHandler = new PaginationHandler();

  private readonly _apiUrl = environment.apiUrl + 'api/like/';

  create(targetMemberUserName: string): Observable<ApiResponse> {
    return this._http.post<ApiResponse>(this._apiUrl + 'add/' + targetMemberUserName, null);
  }

  delete(unfollowedMember: string): Observable<ApiResponse> {
    return this._http.delete<ApiResponse>(this._apiUrl + 'remove/' + unfollowedMember);
  }

  getAll(likeParams: LikeParams): Observable<PaginatedResult<Member[]>> {
    let params = new HttpParams();

    if (likeParams) {
      params = params.append('pageNumber', likeParams.pageNumber);
      params = params.append('pageSize', likeParams.pageSize);
      params = params.append('predicate', likeParams.predicate);
    }

    return this.paginationHandler.getPaginatedResult<Member[]>(this._apiUrl, params);
  }
}
