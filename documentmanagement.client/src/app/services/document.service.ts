import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { paginatedDocumentResponse } from '../Models/paginatedDocumentResponse';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {

  private baseUrl = environment.documentServiceUri;
  private uploadSuccessSubject = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) { }

  uploadFile(selectedFiles: File[]): Observable<any> {

    const formData: FormData = new FormData();
    selectedFiles.forEach(file => {
      formData.append('files', file, file.name);
    });

    return this.http.post(`${this.baseUrl}/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map((event: any) => {
        if (event.type === HttpEventType.UploadProgress) {
        } else if (event instanceof HttpResponse) {
          this.uploadSuccessSubject.next(true);
        }
        return event;
      })
    );
  }


  downloadFile(documentId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/download/${documentId}`, { responseType: 'blob' });
  }

  onUploadSuccess(): Observable<boolean> {
    return this.uploadSuccessSubject.asObservable();
  }


  GetDocuments(page: number, pageSize: number): Observable<paginatedDocumentResponse> {
    const params = new HttpParams().set('page', page.toString()).set('pageSize', pageSize.toString());
    return this.http.get<paginatedDocumentResponse>(this.baseUrl, { params });
  }
}
