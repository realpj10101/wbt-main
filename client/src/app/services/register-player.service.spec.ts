import { TestBed } from '@angular/core/testing';

import { RegisterPlayerService } from './register-player.service';

describe('RegisterPlayerService', () => {
  let service: RegisterPlayerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegisterPlayerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
