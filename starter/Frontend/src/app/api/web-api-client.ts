import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

/**
 * Manual API client for the flight planning WebApi.
 * 
 * NOTE: This is a manual implementation providing type-safe access to the API.
 * It will remain stable even when running `npm run generate-web-api`.
 * 
 * TODO for students: When the backend is fully implemented, you can optionally:
 * 1. Ensure WebApi is running: dotnet run --project WebApi
 * 2. Download the OpenAPI spec: curl http://localhost:5000/openapi/v1.json > Frontend/WebApi.json
 * 3. Generate client code: npm run generate-web-api
 * 4. Use generated services from '../api-generated' if preferred
 * 
 * For now, this manual client provides all methods needed by the Angular components.
 */
@Injectable({
  providedIn: 'root'
})
export class WebApiClient {
  private http = inject(HttpClient);
  private baseUrl = '/api';

  /**
   * Get route comparison data
   */
  async getRouteComparison(): Promise<RouteComparisonResponse[]> {
    return await firstValueFrom(
      this.http.get<RouteComparisonResponse[]>(`${this.baseUrl}/routes/compare`)
    );
  }

  /**
   * Get configuration recommendation for a route
   */
  async recommendConfiguration(routeCode: string): Promise<ConfigurationRecommendationResponse> {
    return await firstValueFrom(
      this.http.post<ConfigurationRecommendationResponse>(
        `${this.baseUrl}/configuration/recommend`,
        { routeCode }
      )
    );
  }

  /**
   * Validate a flight schedule
   */
  async validateSchedule(flights: ScheduleFlightRequest[]): Promise<ScheduleValidationResponse> {
    return await firstValueFrom(
      this.http.post<ScheduleValidationResponse>(
        `${this.baseUrl}/schedule/validate`,
        { flights }
      )
    );
  }

  /**
   * Analyze booking pattern for a flight
   */
  async analyzeBookings(flightId: number): Promise<BookingAnalysisResponse> {
    return await firstValueFrom(
      this.http.get<BookingAnalysisResponse>(`${this.baseUrl}/bookings/analyze/${flightId}`)
    );
  }
}

// Response types matching WebApi DTOs
export interface RouteComparisonResponse {
  routeCode: string;
  totalFlights: number;
  averageLoadFactor: number;
  averageRevenue: number;
  averageCost: number;
  averageProfit: number;
  recommendedConfiguration: number;
}

export interface ConfigurationRecommendationResponse {
  recommendedCapacity: number;
  expectedLoadFactor: number;
  reasoning: string;
  comparison220?: ConfigurationComparison;
  comparison240?: ConfigurationComparison;
}

export interface ConfigurationComparison {
  capacity: number;
  expectedLoadFactor: number;
  expectedRevenue: number;
  expectedCosts: number;
  expectedProfit: number;
}

export interface ScheduleValidationResponse {
  isValid: boolean;
  issues: string[];
}

export interface ScheduleFlightRequest {
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  aircraftId: string;
  departureAirport: string;
  arrivalAirport: string;
}

export interface BookingAnalysisResponse {
  flightId: number;
  flightNumber: string;
  totalBookings: number;
  earlyBirdCount: number;
  earlyBirdPercentage: number;
  standardCount: number;
  standardPercentage: number;
  lastMinuteCount: number;
  lastMinutePercentage: number;
  averageBookingPrice: number;
  revenueOptimizationScore: number;
}
