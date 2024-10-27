import { isPlatformBrowser } from '@angular/common';
import { HttpInterceptorFn } from '@angular/common/http';
import { PLATFORM_ID, inject } from '@angular/core';
import { LoggedInPlayer } from '../models/logged-in-player.model';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const plarfromId = inject(PLATFORM_ID);

  if (isPlatformBrowser(plarfromId)) {
    const loggedInPlayerStr: string | null = localStorage.getItem('loggedInPlayer');

    if (loggedInPlayerStr) {
      const loggedInPlayer: LoggedInPlayer = JSON.parse(loggedInPlayerStr);

      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${loggedInPlayer.token}`
        }
      });
    }
  }

  return next(req);
};
