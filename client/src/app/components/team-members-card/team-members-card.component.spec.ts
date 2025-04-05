import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMembersCardComponent } from './team-members-card.component';

describe('TeamMembersCardComponent', () => {
  let component: TeamMembersCardComponent;
  let fixture: ComponentFixture<TeamMembersCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeamMembersCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeamMembersCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
