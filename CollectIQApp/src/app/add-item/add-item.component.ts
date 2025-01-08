import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatIcon } from '@angular/material/icon';
import { ItemService } from '../services/item.service';
import { ItemTypeService } from '../services/item-type.service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { DecodedToken } from '../models/decoded-token.model';
import { tap } from 'rxjs/operators';
import { BarcodeScannerComponent } from '../barcode-scanner/barcode-scanner.component';
import { ItemType } from '../models/item-type.model';

@Component({
  selector: 'app-add-item',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDividerModule,
    MatIcon
  ],
  templateUrl: './add-item.component.html',
  styleUrl: './add-item.component.scss'
})
export class AddItemComponent implements OnInit{
  itemForm: FormGroup;  
  selectedItemType: string = '';
  userInfo: DecodedToken =  new DecodedToken('', '', '', '', 0, 0, 0, '');
  itemTypes: ItemType[] = [];

  constructor(private fb: FormBuilder, 
    private itemService: ItemService,
    private itemTypeService: ItemTypeService, 
    private router: Router, 
    private dialog: MatDialog, 
    private dialogRef: MatDialogRef<AddItemComponent>, 
    @Inject(MAT_DIALOG_DATA) public data: { itemType: string }) {

    
    this.itemForm = this.fb.group({
      name: ['', Validators.required],
      brand: ['', Validators.required],
      description: [''],
      dateAcquired: ['', Validators.required],
      price: ['', Validators.required],
      serialNumber: [''],
      imageUrl: [''],
      itemTypeId: [this.selectedItemType, Validators.required],
      userId: [this.userInfo.UserId] // Populate with the actual user ID
    });
  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.userInfo = jwtDecode<DecodedToken>(token);
      this.itemForm.patchValue({ userId: this.userInfo.UserId});
    }

    this.itemTypeService.getItemTypes().subscribe(itemTypes => {
      this.itemTypes = itemTypes;
      this.selectedItemType = this.itemTypes.find(itemType => itemType.id === this.data.itemType)?.name || '';
      this.itemForm.patchValue({ itemTypeId: this.data.itemType });
      this.addTypeSpecificFields();
    });    
  }

  onItemTypeChange() {
    this.selectedItemType = this.itemForm.get('itemTypeId')?.value;
    this.addTypeSpecificFields();
  }

  openScanner(): void{
    const dialogRef = this.dialog.open(BarcodeScannerComponent, {
      width: '80%',
      height: '80%'
    });

    dialogRef.afterClosed().subscribe((scannedData) => {
      if(scannedData){        
        this.itemForm.patchValue({ serialNumber: scannedData});
        this.fetchBarcodeData(scannedData);
      }
    });
  }

  fetchBarcodeData(barcode: string): void {
    this.itemService.getItemFromBarcode(barcode).subscribe(
      (response) => {
        if (response) {
          // Pre-fill form based on response
          this.itemForm.patchValue({
            name: response[0].name,
            serialNumber: response[0].ean
          });
        }
      },
      (error) => {
        console.error('Error fetching product details:', error);
        alert('No product details found for this barcode.');
      }
    );
  }

  addTypeSpecificFields() {
    // Remove all type-specific fields first
    this.removeTypeSpecificFields();

    // Add fields based on selected item type
    if (this.selectedItemType === 'Watch') {
      this.itemForm.addControl('movementType', this.fb.control(''));
      this.itemForm.addControl('caseMaterial', this.fb.control(''));
      this.itemForm.addControl('caseDiameter', this.fb.control(''));
      this.itemForm.addControl('caseThickness', this.fb.control(''));
      this.itemForm.addControl('bandMaterial', this.fb.control(''));
      this.itemForm.addControl('bandWidth', this.fb.control(''));
    } else if (this.selectedItemType === 'Cologne') {
      this.itemForm.addControl('fragranceNotes', this.fb.control(''));
      this.itemForm.addControl('concentration', this.fb.control(''));
      this.itemForm.addControl('type', this.fb.control(''));
      this.itemForm.addControl('size', this.fb.control(''));
    } else if (this.selectedItemType === 'Sneaker') {
      this.itemForm.addControl('colorway', this.fb.control(''));
      this.itemForm.addControl('size', this.fb.control(''));
    }
  }



  removeTypeSpecificFields() {
    ['movementType', 'caseMaterial', 'caseDiameter', 'caseThickness', 'bandMaterial', 'bandWidth', 'fragranceNotes', 'concentration', 'type', 'size', 'colorway']
      .forEach(field => this.itemForm.removeControl(field));
  }

  onSubmit() {
    if (this.itemForm.valid) {
      this.itemService.createItem(this.itemForm.value).pipe(
            tap(response => {
              console.log('Item created successfully', response);              
              this.dialogRef.close();
              this.router.navigate(['/items', this.data.itemType]);
            })
          ).subscribe({
            next: () => {},
            error: error => {
              console.error('Error creating item', error);
            }
          });
    }
  }
}
