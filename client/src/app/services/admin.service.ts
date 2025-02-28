import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { UserWithRole } from '../models/user-with-role.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private _http = inject(HttpClient);

  private readonly _apiUrl = environment.apiUrl + 'api/admin/'

  getUsersWithRoles(): Observable<UserWithRole[]>  {
    return this._http.get<UserWithRole[]>(this._apiUrl + 'users-with-roles'); 
  }

}
