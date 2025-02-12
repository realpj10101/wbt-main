import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Member } from '../models/member.model';
import { UserUpdate } from '../models/user-update.model';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/helpers/apiResponse.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  http = inject(HttpClient);

  members: Member[] = [];
  private readonly apiUrl = environment.apiUrl + 'api/playeruser';

  updateUser(userUpdate: UserUpdate): Observable<ApiResponse> {
    console.log("ok")
    return this.http.put<ApiResponse>(this.apiUrl, userUpdate);
  }

  setMainPhoto(url_165In: string): Observable<ApiResponse> {
    let queryParams = new HttpParams().set('photoUrlIn', url_165In);

    return this.http.put<ApiResponse>(this.apiUrl + 'set-main-photo', null, { params: queryParams});
  }

  deletePhoto(url_165In: string): Observable<ApiResponse> {
    let queryParams = new HttpParams().set('photoUrlIn', url_165In);

    return this.http.put<ApiResponse>(this.apiUrl + 'delete-photo', null, { params: queryParams});
  }
}
