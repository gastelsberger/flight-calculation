import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebApiClient } from '../api/web-api-client';

@Component({
  selector: 'app-configuration-optimizer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './configuration-optimizer.html',
  styleUrl: './configuration-optimizer.css'
})
export class ConfigurationOptimizer {
  private apiClient = inject(WebApiClient);

  routeCode = signal('');
  recommendation = signal<ConfigurationRecommendation | null>(null);
  error = signal<string | null>(null);
  isLoading = signal(false);

  onSubmit() {
    // TODO: Students implement
    // 1. Validate routeCode format (7 characters, XXX-XXX pattern)
    // 2. Call API to get recommendation
    // 3. Display results or error
    
    // Stub implementation
    console.log('TODO: Implement onSubmit');
  }

  getSpillage(capacity: number): number {
    // TODO: Students implement
    // Calculate spillage based on average demand and capacity
    // Spillage = max(0, averageDemand - capacity)
    // Hint: You need to extract average demand from the recommendation data
    
    // Stub implementation
    return 0;
  }

  getLoadFactorForCapacity(capacity: number): number {
    // TODO: Students implement
    // Calculate load factor for a given capacity
    // Load Factor = averageDemand / capacity
    
    // Stub implementation
    return 0;
  }

  formatLoadFactor(loadFactor: number): string {
    const percentage = (loadFactor * 100).toFixed(1);
    return loadFactor > 1.0 ? `${percentage}%*` : `${percentage}%`;
  }
}

interface ConfigurationRecommendation {
  recommendedCapacity: number;
  expectedLoadFactor: number;
  reasoning: string;
}
