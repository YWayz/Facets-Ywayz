import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportVisitorListComponent } from './report-visitor-list.component';

describe('ReportVisitorListComponent', () => {
  let component: ReportVisitorListComponent;
  let fixture: ComponentFixture<ReportVisitorListComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ReportVisitorListComponent]
    });
    fixture = TestBed.createComponent(ReportVisitorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
