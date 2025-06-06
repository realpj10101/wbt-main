import { TestBed } from '@angular/core/testing';

import { TeamMessagingService } from './team-messaging.service';

describe('TeamMessagingService', () => {
  let service: TeamMessagingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TeamMessagingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
