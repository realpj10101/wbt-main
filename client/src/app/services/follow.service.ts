import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PaginationHandler } from '../extension/paginationHandler';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model';
import { FollowParams } from '../models/helpers/follow-params.model';
import { PaginatedResult } from '../models/helpers/pagination-result.model';
import { Member } from '../models/member.model';

@Injectable({
  providedIn: 'root'
})
export class FollowService {
  private readonly _http = inject(HttpClient);

  private paginationHandler = new PaginationHandler();

  private readonly _apiUrl = environment.apiUrl + 'api/follow/';

  create(targetMemberUserName: string): Observable<ApiResponse> {
    return this._http.post<ApiResponse>(this._apiUrl + 'add-follow/' + targetMemberUserName, null);
  }

  getAll(followParams: FollowParams): Observable<PaginatedResult<Member[]>> {
    let params = new HttpParams();

    if (followParams) {
      params = params.append('pageNumber', followParams.pageNumber);
      params = params.append('pageSize', followParams.pageSize);
      params = params.append('predicate', followParams.predicate);
    }

    // Use the generic method and make it reusabel for all components.
    return this.paginationHandler.getPaginatedResult<Member[]>(this._apiUrl, params);
  }

  delete(unfollowedMember: string): Observable<ApiResponse> {
    return this._http.delete<ApiResponse>(this._apiUrl + 'remove-follow/' + unfollowedMember);
  }
}
