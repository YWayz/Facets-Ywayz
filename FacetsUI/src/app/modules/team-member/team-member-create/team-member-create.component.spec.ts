import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMemberCreateComponent } from './team-member-create.component';

describe('TeamMemberCreateComponent', () => {
  let component: TeamMemberCreateComponent;
  let fixture: ComponentFixture<TeamMemberCreateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TeamMemberCreateComponent]
    });
    fixture = TestBed.createComponent(TeamMemberCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
