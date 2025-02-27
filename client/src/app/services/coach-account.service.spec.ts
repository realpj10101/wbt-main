import { TestBed } from '@angular/core/testing';

import { CoachAccountService } from './coach-account.service';

describe('CoachAccountService', () => {
  let service: CoachAccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CoachAccountService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
