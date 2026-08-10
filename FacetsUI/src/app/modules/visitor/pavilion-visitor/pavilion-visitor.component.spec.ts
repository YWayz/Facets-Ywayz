import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PavilionVisitorComponent } from './pavilion-visitor.component';

describe('PavilionVisitorComponent', () => {
  let component: PavilionVisitorComponent;
  let fixture: ComponentFixture<PavilionVisitorComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PavilionVisitorComponent]
    });
    fixture = TestBed.createComponent(PavilionVisitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
