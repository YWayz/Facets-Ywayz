import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPermissionViewComponent } from './user-permission-view.component';

describe('UserPermissionViewComponent', () => {
  let component: UserPermissionViewComponent;
  let fixture: ComponentFixture<UserPermissionViewComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UserPermissionViewComponent]
    });
    fixture = TestBed.createComponent(UserPermissionViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
