import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WebApiClient } from '../api/web-api-client';

@Component({
  selector: 'app-booking-timeline',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-timeline.html',
  styleUrl: './booking-timeline.css'
})
export class BookingTimeline {
  private apiClient = inject(WebApiClient);

  flightId = signal<number | null>(null);
  analysis = signal<BookingAnalysis | null>(null);
  error = signal<string | null>(null);
  isLoading = signal(false);

  onSubmit() {
    // TODO: Students implement
    // 1. Validate flightId is a positive number
    // 2. Call API to get booking analysis
    // 3. Handle 404 (flight not found), 204 (no bookings), 200 (success)
    
    // Stub implementation
    console.log('TODO: Implement onSubmit');
  }

  getScoreClass(score: number): string {
    if (score >= 0.85) return 'score-excellent';
    if (score >= 0.70) return 'score-good';
    return 'score-poor';
  }

  getBarWidth(percentage: number): string {
    return `${(percentage * 100).toFixed(1)}%`;
  }
}

interface BookingAnalysis {
  totalBookings: number;
  earlyBirdPercentage: number;
  lastMinutePercentage: number;
  averageTicketPrice: number;
  revenueOptimizationScore: number;
  analysis: string;
}
