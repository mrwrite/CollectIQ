import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ItemService } from '../services/item.service';
import { ItemTypeService } from '../services/item-type.service';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Item } from '../models/item.model';
import { AddItemComponent } from '../add-item/add-item.component';
import { DecodedToken } from '../models/decoded-token.model';
import { jwtDecode } from 'jwt-decode';
import { ItemType } from '../models/item-type.model';

@Component({
  selector: 'app-item-list',
  standalone: true,
  imports: [
    MatTableModule,
    CommonModule,
    MatCardModule,
    MatToolbarModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './item-list.component.html',
  styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent implements OnInit {
  itemType: string = '';
  itemTypeName: string = '';
  items: Item[] = [];
  itemTypes: ItemType[] = [];
  displayedColumns: string[] = ['delete', 'name', 'description', 'dateAcquired', 'price', 'serialNumber'];
   userInfo!: DecodedToken;

  constructor(private route: ActivatedRoute, 
    private itemService: ItemService,
    private itemTypeService: ItemTypeService, 
    private dialog: MatDialog) {}

    ngOnInit(): void {
      const token = localStorage.getItem('token');
      if (token) {
        this.userInfo = jwtDecode<DecodedToken>(token);
      }
    
      this.route.paramMap.subscribe((params) => {
        this.itemType = params.get('type') || '';
      });
    
      this.itemTypeService.getItemTypes().subscribe((itemTypes) => {
        this.itemTypes = itemTypes;
        this.itemTypeName = this.itemTypes.find((itemType) => itemType.id === this.itemType)?.name || '';
        console.log('Resolved itemTypeName:', this.itemTypeName); // Debug log
        this.buildColumns(this.itemTypeName); // Ensure columns are built here
        this.loadItems();
      });
    }
    

  loadItems(): void {
    this.itemService.getItemsByUserId(this.userInfo.UserId).subscribe(items => {
      this.items = this.itemTypeName == "" ? items : items.filter(item => item.itemTypeName === this.itemTypeName);
      console.log('Items:', this.items); // Debug log
      console.log('Displayed columns:', this.displayedColumns); // Debug log
    });
  }

  buildColumns(typeName: string): void{
    this.displayedColumns = ['delete', 'name', 'description', 'dateAcquired', 'price', 'serialNumber' ];

    switch (typeName) {
      case 'Watch':
        this.displayedColumns.push('movementType', 'caseMaterial', 'caseDiameter', 'caseThickness', 'bandMaterial', 'bandWidth');
        break;
      case 'Cologne':
        this.displayedColumns.push('fragranceNotes', 'concentration', 'type');
        break;
      case 'Sneaker':
        this.displayedColumns.push('colorway', 'size');
        break;
      default:              
        this.displayedColumns.push('itemType');
        break;
    }

    console.log('Building columns for type:', typeName);
    console.log('Displayed columns:', this.displayedColumns);

  }


  deleteItem(itemId: string): void{
    this.itemService.deleteItem(itemId).subscribe(() => {
      this.loadItems();
    });
  }

  openAddItemDialog(itemType: string): void {
    this.dialog.open(AddItemComponent, {
      width: '800px',
      data: { itemType: this.itemType }
    });
  } 

}
