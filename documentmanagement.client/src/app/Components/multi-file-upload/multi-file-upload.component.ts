import { Component } from '@angular/core';
import { DocumentService } from 'src/app/services/document.service';

@Component({
  selector: 'app-multi-file-upload',
  templateUrl: './multi-file-upload.component.html',
  styleUrls: ['./multi-file-upload.component.css']
})
export class MultiFileUploadComponent {
  selectedFiles: File[] = [];
  uploadStatus: number = 0;

  constructor(private documentService: DocumentService) { }

  onFileSelected(event: any): void {
    this.selectedFiles = Array.from(event.target.files);
    this.uploadStatus = 0;
  }

  uploadFiles(): void {

    this.documentService.uploadFile(this.selectedFiles).subscribe(
      {
        next: () => {
          this.uploadStatus = 1;
        },
        error: () => {
          this.uploadStatus = 2;
        }
      }
    );

  }
}
