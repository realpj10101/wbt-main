import { TestBed } from '@angular/core/testing';

import { PlayerUserService } from './player-user.service';

describe('PlayerUserService', () => {
  let service: PlayerUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PlayerUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
