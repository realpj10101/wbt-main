import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoachPanelCardComponent } from './coach-panel-card.component';

describe('CoachPanelCardComponent', () => {
  let component: CoachPanelCardComponent;
  let fixture: ComponentFixture<CoachPanelCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoachPanelCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoachPanelCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
