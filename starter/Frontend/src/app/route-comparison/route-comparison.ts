import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebApiClient } from '../api/web-api-client';

@Component({
  selector: 'app-route-comparison',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './route-comparison.html',
  styleUrl: './route-comparison.css'
})
export class RouteComparison {
  private apiClient = inject(WebApiClient);

  routes = signal<RouteComparisonData[]>([]);
  filterText = signal('');
  sortColumn = signal<string>('averageProfit');
  sortAscending = signal(false);

  // TODO: Students implement computed signals for filtering and sorting
  filteredRoutes = computed(() => {
    // Filter routes based on filterText (substring match on routeCode, case-insensitive)
    // Then sort based on sortColumn and sortAscending
    // Return the filtered and sorted array
    
    // Stub implementation - students should replace this
    return [] as RouteComparisonData[];
  });

  // TODO: Students implement computed signal for summary statistics
  summaryStats = computed(() => {
    // Calculate summary from filteredRoutes:
    // - totalFlights (sum across all routes)
    // - averageLoadFactor (weighted by number of flights, not simple average!)
    // - totalProfit (sum across all routes)
    
    // Stub implementation - students should replace this
    return {
      totalFlights: 0,
      averageLoadFactor: 0,
      totalProfit: 0
    };
  });

  async ngOnInit() {
    await this.loadRoutes();
  }

  async loadRoutes() {
    // TODO: Students implement
    // Call apiClient to get route comparison data
    // Update routes signal with the result
    
    // Stub implementation
    console.log('TODO: Implement loadRoutes');
  }

  onFilterChange(value: string) {
    this.filterText.set(value);
  }

  onSort(column: string) {
    // TODO: Students implement
    // If clicking the same column, toggle sort direction
    // If clicking a different column, set that column and default to descending
    
    // Stub implementation
    console.log('TODO: Implement onSort', column);
  }

  getSortIndicator(column: string): string {
    if (this.sortColumn() !== column) return '';
    return this.sortAscending() ? '↑' : '↓';
  }

  getProfitClass(profit: number): string {
    return profit >= 0 ? 'profit-positive' : 'profit-negative';
  }
}

interface RouteComparisonData {
  routeCode: string;
  totalFlights: number;
  averageLoadFactor: number;
  averageRevenue: number;
  averageCost: number;
  averageProfit: number;
  recommendedConfiguration: number;
}
