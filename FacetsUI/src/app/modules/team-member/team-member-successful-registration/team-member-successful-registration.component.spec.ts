import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMemberSuccessfulRegistrationComponent } from './team-member-successful-registration.component';

describe('TeamMemberSuccessfulRegistrationComponent', () => {
  let component: TeamMemberSuccessfulRegistrationComponent;
  let fixture: ComponentFixture<TeamMemberSuccessfulRegistrationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TeamMemberSuccessfulRegistrationComponent]
    });
    fixture = TestBed.createComponent(TeamMemberSuccessfulRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
