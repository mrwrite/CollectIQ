import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ItemTypeService {
  private apiUrl = `${environment.apiUrl}/itemtype`;  

  constructor(private http: HttpClient) { }

  getItemTypes(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`);
  }
}
