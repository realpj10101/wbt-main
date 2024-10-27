import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';
import { Observable, map, take } from 'rxjs';
import { RegisterPlayer } from '../models/register-player.model';
import { LoginPlayer } from '../models/login-player.model';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { LoggedInPlayer } from '../models/logged-in-player.model';
import { ApiResponse } from '../models/helpers/apiResponse.model';

@Injectable({
  providedIn: 'root'
})
export class RegisterPlayerService {
  //#region injects and variables
  http = inject(HttpClient);
  router = inject(Router);
  platformId = inject(PLATFORM_ID);
}
