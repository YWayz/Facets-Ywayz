import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamMemberAttachmentComponent } from './team-member-attachment.component';

describe('TeamMemberAttachmentComponent', () => {
  let component: TeamMemberAttachmentComponent;
  let fixture: ComponentFixture<TeamMemberAttachmentComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TeamMemberAttachmentComponent]
    });
    fixture = TestBed.createComponent(TeamMemberAttachmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
