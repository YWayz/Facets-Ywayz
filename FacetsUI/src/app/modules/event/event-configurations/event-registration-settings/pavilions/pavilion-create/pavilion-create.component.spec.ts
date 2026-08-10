import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PavilionCreateComponent } from './pavilion-create.component';

describe('PavilionCreateComponent', () => {
  let component: PavilionCreateComponent;
  let fixture: ComponentFixture<PavilionCreateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PavilionCreateComponent]
    });
    fixture = TestBed.createComponent(PavilionCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
