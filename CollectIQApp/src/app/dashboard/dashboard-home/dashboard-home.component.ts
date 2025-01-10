import { AfterViewInit, Component, OnInit, ViewEncapsulation } from '@angular/core';
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
import { Item } from '../../models/item.model';

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
  styleUrl: './dashboard-home.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class DashboardHomeComponent implements OnInit, AfterViewInit {
  items: ItemStats[] = [];
  rawItems: Item[] = [];
  recentItems: Item[] = [];
  recentYearItems: Item[] = [];
  displayedColumns: string[] = ['name', 'itemTypeName', 'price', 'dateAcquired'];
  yScaleMax: number = 0;
  userInfo!: DecodedToken;
  chartData: any[] = [];
  lineChartData: any[] = [];
  totalItemCount: number = 0;
  totalSpending: number = 0;
  totalItemGrowth: number = 55; // Mock growth percentage
  totalSpendingGrowth: number = 45; // Mock growth percentage
  
  isSmallScreen = false;
  gridCols = 3;
  view: [number, number] = [700, 200];
  colorScheme: Color = {
    name: 'customScheme',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#1171BA']
  };
  customColors = [
    { name: 'tickLabel', value: '#afafaf' } // Set the color to your desired hex value
  ];
  

  constructor(private itemService: ItemService, private router: Router
    , private breakpointObserver: BreakpointObserver
  ) {}
  ngAfterViewInit(): void {
     // Delay to ensure the chart is rendered
     setTimeout(() => {
      const xTicks = document.querySelectorAll('g.axis.x text');
      const yTicks = document.querySelectorAll('g.axis.y text');

      // Apply styles to X-axis ticks
      xTicks.forEach((tick) => {
        (tick as HTMLElement).setAttribute('fill', '#666666');
      });

      // Apply styles to Y-axis ticks
      yTicks.forEach((tick) => {
        (tick as HTMLElement).setAttribute('fill', '#666666');
      });
    });
  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.userInfo = jwtDecode<DecodedToken>(token);
    }

    this.itemService.getItemsByUserId(this.userInfo.UserId).subscribe(items => {
      this.rawItems = items;
      this.items = this.groupItemsByType(items);
      this.prepareChartData();
      this.prepareLineChartData(this.rawItems);

      // Calculate totals
      this.totalItemCount = items.length;
      this.totalSpending = items.reduce((sum, item) => sum + (item.price || 0), 0);
      
      this.recentItems = items.sort((a, b) => new Date(b.dateAcquired).getTime() - new Date(a.dateAcquired).getTime()).slice(0, 4);
      
      // Determine the most recent distinct year dynamically
      const distinctYears = [...new Set(items.map(item => new Date(item.dateAcquired).getFullYear()))];
      const mostRecentYear = Math.max(...distinctYears);

      this.recentYearItems = items.filter(item => {
        const year = new Date(item.dateAcquired).getFullYear();
        return year === mostRecentYear;
      });
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
  

  tickFormatter(value: string | number): string {
    return `<span style="color: #afafaf;">${value}</span>`; // Apply inline styling
  }
  

  private prepareChartData(): void {
    this.yScaleMax = Math.max(...this.items.map(item => item.count)) + 1;
    this.chartData = this.items.map(item => ({
      name: item.type,
      value: item.count
    }));
  }

  private prepareLineChartData(items: Item[]): void {
    const yearlyTotals = items.reduce((acc, item) => {
      const year = new Date(item.dateAcquired).getFullYear();
      acc[year] = (acc[year] || 0) + item.price;
      return acc;
    }, {} as Record<number, number>);
  
    this.lineChartData = [
      {
        name: 'Total Spending',
        series: Object.entries(yearlyTotals).map(([year, total]) => ({
          name: year,
          value: total || 0, // Fallback to 0 for null/undefined values
        })),
      },
    ];
  
    console.log('Prepared Line Chart Data:', this.lineChartData);
  }
  

  logout(): void {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  navigateToItemList(type: string): void {
    this.router.navigate(['/items', type], { queryParams: { type } });
  }

}
