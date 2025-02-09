import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { PaginationHandler } from '../extension/paginationHandler';
import { MemberParams } from '../models/helpers/member-params.model';
import { empty, Observable } from 'rxjs';
import { PaginatedResult } from '../models/helpers/pagination-result.model';
import { Member } from '../models/member.model';
import { P } from '@angular/cdk/keycodes';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http = inject(HttpClient);

  private readonly _baseApiUrl = environment.apiUrl + 'member/'
  private paginationHandler = new PaginationHandler();

  getAll(memberParams: MemberParams): Observable<PaginatedResult<Member[]>> {
    const params = this.getHttpParams(memberParams);

    return this.paginationHandler.getPaginatedResult<Member[]>(this._baseApiUrl, params);
  }

  getByUserName(userNameInput: string): Observable<Member | undefined> {
    return this.http.get<Member>(this._baseApiUrl + 'get-by-username/' + userNameInput);
  }

  private getHttpParams(memberParams: MemberParams): HttpParams {
    let params = new HttpParams();

    if (memberParams) {
      params = params.append('pageSize', memberParams.pageSize);
      params = params.append('pageNumebr', memberParams.pageNumber);
      params = params.append('orderBy', memberParams.orderBy);
    }
    
    return params;
  }
}
