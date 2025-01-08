import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Item } from '../models/item.model';
import { CreateItemDto } from '../models/create-item.model';
import * as changeCase from 'change-case-object';
import { A11yModule } from '@angular/cdk/a11y';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  private apiUrl = `${environment.apiUrl}/item`;
  private barcodeApiUrl = `${environment.barcodeApiUrl}`;
  private apiKey = `${environment.apiKey}`;

  constructor(private http: HttpClient) {}

  getItemStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`);
  }

   getItemsByUserId(userId: string): Observable<Array<Item>> {
    return this.http.get<Array<Item>>(`${this.apiUrl}/user/${userId}`);
   }

    getItemsByType(type: string): Observable<Array<Item>> {
     return this.http.get<Array<Item>>(`${this.apiUrl}/type/${type}`);
    }

    createItem(itemDto : CreateItemDto): Observable<any> {       
        return this.http.post<any>(`${this.apiUrl}`, itemDto );
    }

    getItemFromBarcode(barcode: string): Observable<any>{
      const params = new HttpParams()
        .set('token', this.apiKey)
        .set('op', 'barcode-lookup')
        .set('ean', barcode);

        return this.http.get<any>(this.barcodeApiUrl, { params });
    }

    deleteItem(itemId: string): Observable<any> {
      return this.http.delete<any>(`${this.apiUrl}/${itemId}`);
    }
}
