import { Routes } from '@angular/router';
import { DummiesList } from './dummies-list/dummies-list';
import { GenerateRecords } from './generate-records/generate-records';
import { RouteComparison } from './route-comparison/route-comparison';
import { ConfigurationOptimizer } from './configuration-optimizer/configuration-optimizer';
import { BookingTimeline } from './booking-timeline/booking-timeline';

export const routes: Routes = [
    { path: 'routes', component: RouteComparison },
    { path: 'optimizer', component: ConfigurationOptimizer },
    { path: 'bookings', component: BookingTimeline },
    { path: 'dummies', component: DummiesList },
    { path: 'generate', component: GenerateRecords },
    { path: '', redirectTo: '/routes', pathMatch: 'full' }
];
