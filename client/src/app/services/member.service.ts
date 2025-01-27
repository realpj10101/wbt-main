import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { PaginationHandler } from '../extension/paginationHandler';
import { MemberParams } from '../models/helpers/member-params.model';
import { Observable } from 'rxjs';
import { PaginationResult } from '../models/helpers/pagination-result.model';
import { Member } from '../models/member.model';
import { P } from '@angular/cdk/keycodes';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private http = inject(HttpClient);

  private readonly _baseApiUrl = environment.apiUrl + 'member/'
  private paginationHandler = new PaginationHandler();

  getAll(memberParams: MemberParams): Observable<PaginationResult<Member[]>> {
    let params = new HttpParams();

    if (memberParams) {
      params = params.append('pageNumber', memberParams.pageNumber);
      params = params.append('pageSize', memberParams.pageSize);
    }

    return this.paginationHandler.getPaginationResult<Member[]>(this._baseApiUrl, params);
  }

  getByUserName(userNameInput: string): Observable<Member | undefined> {
    return this.http.get<Member>(this._baseApiUrl + 'get-by-username/' + userNameInput);
  }
}
