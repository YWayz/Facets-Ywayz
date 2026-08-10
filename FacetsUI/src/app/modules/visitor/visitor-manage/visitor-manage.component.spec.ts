import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorManageComponent } from './visitor-manage.component';

describe('VisitorManageComponent', () => {
  let component: VisitorManageComponent;
  let fixture: ComponentFixture<VisitorManageComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorManageComponent]
    });
    fixture = TestBed.createComponent(VisitorManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
