import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMemberActivityComponent } from './team-member-activity.component';

describe('TeamMemberActivityComponent', () => {
  let component: TeamMemberActivityComponent;
  let fixture: ComponentFixture<TeamMemberActivityComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TeamMemberActivityComponent]
    });
    fixture = TestBed.createComponent(TeamMemberActivityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
