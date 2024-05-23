import { Component, OnInit } from '@angular/core';
import { CsrfTokenService } from 'src/app/services/csrf-token.service';

@Component({
  selector: 'app-document-management',
  templateUrl: './document-management.component.html',
  styleUrls: ['./document-management.component.css']
})
export class DocumentManagementComponent implements OnInit {

  constructor(private csrfTokenService: CsrfTokenService) { }

  ngOnInit(): void {
    this.csrfTokenService.fetchCsrfToken();
  }

}
