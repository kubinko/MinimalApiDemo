import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Attendee } from '../models/attendee';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) { }

  addAttendee(body: Attendee): Observable<any> {
    return this.http.post(`${environment.apiUrl}/attendance`, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  getAttendees(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/attendance`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getInvoiceUri(id: number, code: string): string {
    return `${environment.apiUrl}/attendance/${id}/invoice?code=${code}`;
  }

  deleteAttendee(id: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/attendance/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  getWorkshopInfo(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/info`)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: any) {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}
