import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassCategoryCreateComponent } from './pass-category-create.component';

describe('PassCategoryCreateComponent', () => {
  let component: PassCategoryCreateComponent;
  let fixture: ComponentFixture<PassCategoryCreateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassCategoryCreateComponent]
    });
    fixture = TestBed.createComponent(PassCategoryCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
