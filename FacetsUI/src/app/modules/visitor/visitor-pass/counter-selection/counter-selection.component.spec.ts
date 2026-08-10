import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CounterSelectionComponent } from './counter-selection.component';

describe('CounterSelectionComponent', () => {
  let component: CounterSelectionComponent;
  let fixture: ComponentFixture<CounterSelectionComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CounterSelectionComponent]
    });
    fixture = TestBed.createComponent(CounterSelectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
