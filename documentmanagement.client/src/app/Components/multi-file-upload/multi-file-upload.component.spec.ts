import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MultiFileUploadComponent } from './multi-file-upload.component';

describe('MultiFileUploadComponent', () => {
  let component: MultiFileUploadComponent;
  let fixture: ComponentFixture<MultiFileUploadComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MultiFileUploadComponent]
    });
    fixture = TestBed.createComponent(MultiFileUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
