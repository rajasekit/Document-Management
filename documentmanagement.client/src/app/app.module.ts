import { HTTP_INTERCEPTORS, HttpClientModule, HttpClientXsrfModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { CsrfInterceptor } from './csrf-interceptor';
import { MultiFileUploadComponent } from './Components/multi-file-upload/multi-file-upload.component';
import { DocumentManagementComponent } from './Components/document-management/document-management.component';
import { DocumentListComponent } from './Components/document-list/document-list.component';

@NgModule({
  declarations: [
    AppComponent,
    MultiFileUploadComponent,
    DocumentManagementComponent,
    DocumentListComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    HttpClientXsrfModule.withOptions({
      cookieName: 'XSRF-TOKEN',
      headerName: 'X-XSRF-TOKEN'
    })
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CsrfInterceptor,
      multi: true
    }

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
