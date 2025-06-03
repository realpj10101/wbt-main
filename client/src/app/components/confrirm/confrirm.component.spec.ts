import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfrirmComponent } from './confrirm.component';

describe('ConfrirmComponent', () => {
  let component: ConfrirmComponent;
  let fixture: ComponentFixture<ConfrirmComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfrirmComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfrirmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
