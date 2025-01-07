import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common'
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { BrowserMultiFormatReader, BarcodeFormat, IScannerControls } from '@zxing/browser';
import { ItemService } from '../services/item.service'
import { AddItemComponent } from '../add-item/add-item.component';

@Component({
  selector: 'app-barcode-scanner',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './barcode-scanner.component.html',
  styleUrl: './barcode-scanner.component.scss'
})
export class BarcodeScannerComponent implements OnInit {
  @ViewChild('scannerPreview', {static: true}) scannerPreview!: ElementRef<HTMLVideoElement>;
  private codeReader = new BrowserMultiFormatReader();
  private scannerControls: IScannerControls | null = null;
  private scanSound = new Audio('assets/sounds/beep.mp3');


  constructor(private dialogRef: MatDialogRef<BarcodeScannerComponent>, private itemService: ItemService, private dialog: MatDialog) {}

  ngOnInit(): void {
    
    this.startScanner();
  }


  startScanner(): void {
    BrowserMultiFormatReader.listVideoInputDevices()
      .then((devices) => {
        if (devices.length > 0) {
          const backCamera = devices.find((device) =>
            device.label.toLowerCase().includes('back')
          );
          const selectedDeviceId = backCamera?.deviceId || devices[0].deviceId;
          this.codeReader.decodeFromVideoDevice(
            selectedDeviceId,
            this.scannerPreview.nativeElement,
            (result, error) => {
              if (result) {
                console.log('Barcode detected:', result.getText());               
                this.playScanSound();
                this.stopScanner(result.getText());
              }
              else if (error && !(error.name === 'NotFoundException')) {
                console.error(error);
              }
            }
          ).then((controls) => {
            this.scannerControls = controls;
          });
        } else {
          console.error('No video devices found.');
        }
      })
      .catch((err) => console.error('Error accessing video devices:', err));
  }  

  playScanSound(): void{
    this.scanSound.currentTime = 0;
    this.scanSound.play().catch((error) => console.error('Error playing scan sound:', error));
  }

  stopScanner(scannedData?: string): void {
    if(this.scannerControls){
      this.scannerControls.stop();
      this.scannerControls = null;
      this.dialogRef.close(scannedData);
    }
    console.log('Scanner stopped');
  }
}
