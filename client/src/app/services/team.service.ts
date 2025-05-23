import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { CreateTeam } from '../models/create.team.model';
import { map, Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { ShowTeam } from '../models/show-team.model';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { PaginationParams } from '../models/helpers/paginationParams.model';
import { PaginatedResult } from '../models/helpers/pagination-result.model';
import { PaginationHandler } from '../extension/paginationHandler';
import { Member } from '../models/member.model';
import { ApiResponse } from '../models/helpers/apiResponse.model';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  http = inject(HttpClient)
  platformId = inject(PLATFORM_ID);
  router = inject(Router);
  currentTeamSignal = signal<ShowTeam | null>(null);

  private readonly _apiUrl = environment.apiUrl + 'api/team/'
  private paginationHandler = new PaginationHandler();

  create(userInput: CreateTeam): Observable<ShowTeam> {
    return this.http.post<ShowTeam>(this._apiUrl + 'create', userInput).pipe(
      map(res => {
        this.navigateToReturnUrl();

        return res;
      })
    );
  }

  getAll(paginationParams: PaginationParams): Observable<PaginatedResult<ShowTeam[]>> {
    const params = this.getHttpParams(paginationParams);

    return this.paginationHandler.getPaginatedResult<ShowTeam[]>(this._apiUrl, params);
  }

  getByTeamName(userIn: string): Observable<ShowTeam | null> {
    return this.http.get<ShowTeam>(this._apiUrl + 'get-by-name/' + userIn).pipe(
      map(res => {
        if (res) {
          this.setCurrentTeam(res);

          return res;
        }

        return null;
      })
    )
  }

  getTeamMembersAsync(userIn: string): Observable<Member[]> {
    return this.http.get<Member[]>(this._apiUrl + 'get-members/' + userIn);
  }

  addMember(teamName: ApiResponse, targetMemberUserName: string): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this._apiUrl + 'add-member/' + teamName.message + '/' + targetMemberUserName, null);
  }

  getTeamName(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(this._apiUrl + 'get-team-name');
  }

  assignCaptain(targetUserName: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this._apiUrl + 'assign-captain/' + targetUserName, null);
  }

  removeCaptain(targetUserName: string) {
    return this.http.delete<ApiResponse>(this._apiUrl + 'remove-captain/' + targetUserName);
  }

  setMainPhoto(url_165: string, teamName: string): Observable<ApiResponse> {
    let queryParams = new HttpParams().set('photoUrlIn', url_165);

    return this.http.put<ApiResponse>(this._apiUrl + 'set-main-photo/' + teamName, null, { params: queryParams });
  }

  deletePhoto(url_165: string, teamName: string): Observable<ApiResponse> {
    let queryParams = new HttpParams().set('photoUrlIn', url_165);

    return this.http.put<ApiResponse>(this._apiUrl + 'delete-photo/' + teamName, null, { params: queryParams })
  }

  setCurrentTeam(teamDetails: ShowTeam): void {
    this.currentTeamSignal.set(teamDetails);

    if (isPlatformBrowser(this.platformId))
      localStorage.setItem('currentTeam', JSON.stringify(teamDetails));
  }

  private getHttpParams(paginationParams: PaginationParams): HttpParams {
    let params = new HttpParams();

    if (paginationParams) {
      params = params.append('pageSize', paginationParams.pageSize);
      params = params.append('pageNumber', paginationParams.pageNumber);
    }

    return params;
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
