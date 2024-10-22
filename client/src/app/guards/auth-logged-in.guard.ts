import { CanActivateChildFn } from '@angular/router';

export const authLoggedInGuard: CanActivateChildFn = (childRoute, state) => {
  return true;
};
