import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportCollectionComponent } from './report-collection.component';

describe('ReportCollectionComponent', () => {
  let component: ReportCollectionComponent;
  let fixture: ComponentFixture<ReportCollectionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ReportCollectionComponent]
    });
    fixture = TestBed.createComponent(ReportCollectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
