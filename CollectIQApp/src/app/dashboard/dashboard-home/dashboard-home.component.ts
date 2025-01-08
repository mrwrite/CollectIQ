import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ItemService } from '../../services/item.service';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTableModule } from '@angular/material/table';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon'; 
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { NgxChartsModule, Color, ScaleType } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';

import { DecodedToken } from '../../models/decoded-token.model';
import { jwtDecode } from 'jwt-decode';
import { groupBy } from 'lodash';
import { ItemStats } from '../../models/item-stats.model';
import { Router } from '@angular/router';

@Component({
  selector: 'dashboard-home',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatToolbarModule,
    MatTableModule,
    MatGridListModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    MatButtonModule,
    MatMenuModule,
    NgxChartsModule,
  ],
  templateUrl: './dashboard-home.component.html',
  styleUrl: './dashboard-home.component.scss'
})
export class DashboardHomeComponent implements OnInit {
  items: ItemStats[] = [];
  yScaleMax: number = 0;
  userInfo!: DecodedToken;
  chartData: any[] = [];
  isSmallScreen = false;
  gridCols = 3;
  colorScheme: Color = {
    name: 'customScheme',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#1171BA']
  };

  constructor(private itemService: ItemService, private router: Router
    , private breakpointObserver: BreakpointObserver
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.userInfo = jwtDecode<DecodedToken>(token);
    }
    this.itemService.getItemsByUserId(this.userInfo.UserId).subscribe(items => {
      this.items = this.groupItemsByType(items);
      this.prepareChartData();
    });

    this.breakpointObserver.observe([Breakpoints.Small, Breakpoints.XSmall]).subscribe(result => {
      this.isSmallScreen = result.matches;
      this.gridCols = this.isSmallScreen ? 1 : 3;
    });
  }

  private groupItemsByType(items: any[]): ItemStats[] {
    const grouped = groupBy(items, 'itemTypeName');
    return Object.keys(grouped).map(type => {
      const itemsOfType = grouped[type];
      return {
        type,
        itemTypeId: itemsOfType[0]?.itemTypeId || null, // Extract `itemTypeId` from the first item in the group
        count: itemsOfType.length,
        items: itemsOfType,
        showItems: false
      };
    });
  }
  

  private prepareChartData(): void {
    this.yScaleMax = Math.max(...this.items.map(item => item.count)) + 1;
    this.chartData = this.items.map(item => ({
      name: item.type,
      value: item.count
    }));
  }

  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  navigateToItemList(type: string): void {
    this.router.navigate(['/items', type], { queryParams: { type } });
  }

}
