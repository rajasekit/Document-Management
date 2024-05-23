import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CsrfTokenService {
  private csrfToken: string | null = null;

  constructor(private http: HttpClient) { }

  fetchCsrfToken() {
    this.http.get<{ csrfToken: string }>(environment.csrfServiceUri, {withCredentials: true}).subscribe(
      {
        next: (response) => {
          this.csrfToken = response.csrfToken;
        }
      });
  }

  getCsrfToken(): string | null {
    return this.csrfToken;
  }
}
