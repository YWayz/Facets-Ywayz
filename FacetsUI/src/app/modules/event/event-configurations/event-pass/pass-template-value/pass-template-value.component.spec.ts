import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PassTemplateValueComponent } from './pass-template-value.component';

describe('PassTemplateValueComponent', () => {
  let component: PassTemplateValueComponent;
  let fixture: ComponentFixture<PassTemplateValueComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PassTemplateValueComponent]
    });
    fixture = TestBed.createComponent(PassTemplateValueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
