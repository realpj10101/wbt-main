import { TestBed } from '@angular/core/testing';
import { CanActivateChildFn } from '@angular/router';

import { authLoggedInGuard } from './auth-logged-in.guard';

describe('authLoggedInGuard', () => {
  const executeGuard: CanActivateChildFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => authLoggedInGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
