import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisitorPassGenerationComponent } from './visitor-pass-generation.component';

describe('VisitorPassGenerationComponent', () => {
  let component: VisitorPassGenerationComponent;
  let fixture: ComponentFixture<VisitorPassGenerationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VisitorPassGenerationComponent]
    });
    fixture = TestBed.createComponent(VisitorPassGenerationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
