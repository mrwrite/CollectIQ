import { Component, OnInit } from '@angular/core';
import { ItemService } from '../services/item.service';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTableModule } from '@angular/material/table';
import { MatGridListModule } from '@angular/material/grid-list';
import { NgxChartsModule, Color, ScaleType } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';

import { DecodedToken } from '../models/decoded-token.model';
import { jwtDecode } from 'jwt-decode';
import { groupBy } from 'lodash';
import { ItemStats } from '../models/item-stats.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatToolbarModule,
    MatTableModule,
    MatGridListModule,
    NgxChartsModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  items: ItemStats[] = [];
  userInfo!: DecodedToken;
  chartData: any[] = [];
  colorScheme: Color = {
    name: 'customScheme',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#3f51b5', '#e91e63', '#00bcd4', '#ffeb3b']
  };

  constructor(private itemService: ItemService, private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.userInfo = jwtDecode<DecodedToken>(token);
    }
    this.itemService.getItemsByUserId(this.userInfo.UserId).subscribe(items => {
      this.items = this.groupItemsByType(items);
      this.prepareChartData();
    });
  }

  private groupItemsByType(items: any[]): ItemStats[]{
   const grouped = groupBy(items, 'itemTypeName');
   return Object.keys(grouped).map(type => ({
      type,
      count: grouped[type].length,
      items: grouped[type],
      showItems: false
   }));
  }

  private prepareChartData(): void {
    this.chartData = this.items.map(item => ({
      name: item.type,
      value: item.count
    }));
  }

 navigateToItemList(type: string): void {
    this.router.navigate(['/items', type], { queryParams: { type } });
  }
}
