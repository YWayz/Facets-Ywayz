import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMemberPassGenerationComponent } from './team-member-pass-generation.component';

describe('TeamMemberPassGenerationComponent', () => {
  let component: TeamMemberPassGenerationComponent;
  let fixture: ComponentFixture<TeamMemberPassGenerationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TeamMemberPassGenerationComponent]
    });
    fixture = TestBed.createComponent(TeamMemberPassGenerationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
