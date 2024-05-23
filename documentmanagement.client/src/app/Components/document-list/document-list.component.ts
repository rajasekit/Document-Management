import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { document } from 'src/app/Models/document';
import { DocumentService } from 'src/app/services/document.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-document-list',
  templateUrl: './document-list.component.html',
  styleUrls: ['./document-list.component.css']
})
export class DocumentListComponent implements OnInit, OnDestroy {

  message: string = '';
  documents: document[] = [];
  //downloadedFileUrl: string | null = null;
  private uploadSuccessSubscription!: Subscription;

  currentPage: number = 1;
  pageSize: number = environment.gridPageSize;
  totalPages: number = 0;
  totalDocuments: number = 0;

  constructor(private documentService: DocumentService) { }

  ngOnInit(): void {
    this.loadDocuments();
    this.uploadSuccessSubscription = this.documentService.onUploadSuccess().subscribe(() => {
      this.refreshDocuments();
    });
  }

  ngOnDestroy(): void {
    this.uploadSuccessSubscription.unsubscribe();
  }

  loadDocuments(): void {
    this.documentService.GetDocuments(this.currentPage, this.pageSize).subscribe(
      {
        next: (response) => {
          this.documents = response.documents;
          this.totalDocuments = response.totalCount;
        }
      });
  }

  refreshDocuments(): void {
    this.loadDocuments();
  }

  downloadFile(documentId: number, fileName: string): void {
    this.documentService.downloadFile(documentId).subscribe(
      (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        //this.downloadedFileUrl = url;
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      (error: any) => {
        this.message = 'Download failed!';
      }
    );
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadDocuments();
    }
  }

  nextPage(): void {
    this.currentPage++;
    this.loadDocuments();
  }

  goToPage(page: number): void {
    this.currentPage = page;
    this.loadDocuments();
  }

  getPageNumbers(): number[] {
    this.totalPages = Math.ceil(this.totalDocuments / this.pageSize);
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}
