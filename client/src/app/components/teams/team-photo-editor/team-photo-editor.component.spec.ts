import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamPhotoEditorComponent } from './team-photo-editor.component';

describe('TeamPhotoEditorComponent', () => {
  let component: TeamPhotoEditorComponent;
  let fixture: ComponentFixture<TeamPhotoEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeamPhotoEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeamPhotoEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
