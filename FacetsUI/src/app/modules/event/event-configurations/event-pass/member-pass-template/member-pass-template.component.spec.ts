import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberPassTemplateComponent } from './member-pass-template.component';

describe('MemberPassTemplateComponent', () => {
  let component: MemberPassTemplateComponent;
  let fixture: ComponentFixture<MemberPassTemplateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MemberPassTemplateComponent]
    });
    fixture = TestBed.createComponent(MemberPassTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
