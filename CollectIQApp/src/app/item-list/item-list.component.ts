import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ItemService } from '../services/item.service';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { Item } from '../models/item.model';
import { AddItemComponent } from '../add-item/add-item.component';
import { DecodedToken } from '../models/decoded-token.model';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [
    MatTableModule,
    CommonModule,
    MatCardModule,
    MatToolbarModule
  ],
  templateUrl: './item-list.component.html',
  styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent implements OnInit {
  itemType: string = '';
  items: Item[] = [];
  displayedColumns: string[] = ['name', 'description', 'dateAcquired', 'price', 'serialNumber'];
   userInfo!: DecodedToken;

  constructor(private route: ActivatedRoute, private itemService: ItemService, private dialog: MatDialog) {}

  ngOnInit(): void {    

    const token = localStorage.getItem('token');
        if (token) {
          this.userInfo = jwtDecode<DecodedToken>(token);
        }

    this.route.paramMap.subscribe(params => {
      this.itemType = params.get('type') || '';
      switch (this.itemType) {
        case 'Watch':
          this.displayedColumns.push('movementType', 'caseMaterial', 'caseDiameter', 'caseThickness', 'bandMaterial', 'bandWidth');
          break;
        case 'Cologne':
          this.displayedColumns.push('fragranceNotes', 'concentration', 'type');
          break;
        case 'Sneaker':
          this.displayedColumns.push('colorway', 'size');
          break;
      }
      this.loadItems();
    });
  }

  loadItems(): void {
    this.itemService.getItemsByUserId(this.userInfo.UserId).subscribe(items => {
      this.items = items.filter(item => item.itemTypeName === this.itemType);
    });
  }

  openAddItemDialog(itemType: string): void {
    this.dialog.open(AddItemComponent, {
      width: '800px',
      data: { itemType: this.itemType }
    });
  } 

}
