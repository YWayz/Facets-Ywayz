import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PavilionRatesComponent } from './pavilion-rates.component';

describe('PavilionRatesComponent', () => {
  let component: PavilionRatesComponent;
  let fixture: ComponentFixture<PavilionRatesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PavilionRatesComponent]
    });
    fixture = TestBed.createComponent(PavilionRatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
