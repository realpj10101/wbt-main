import { isPlatformBrowser } from '@angular/common';
import { inject, PLATFORM_ID } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const snackbar = inject(MatSnackBar);
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  if (isPlatformBrowser(platformId)) {
    const loggedInPlayerStr: string | null = localStorage.getItem('loggedInUser');

    if (loggedInPlayerStr) {
      return true; // open the component
    }

    // user is not logged-in
    snackbar.open('Please login first.', 'Close', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
      duration: 7000
    });

    localStorage.setItem('returnUrl', state.url);

    router.navigate(['account/login'], { queryParams: { 'returnUrl': state.url } })
  }

  return false; // Block the component
};
