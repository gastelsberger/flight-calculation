# Flight Planning Exercise - Extended Requirements

## Introduction

This exercise extends the basic flight-calculation system into a comprehensive **flight planning and optimization tool**. You will work with historical booking data to optimize aircraft configurations, validate flight schedules, and analyze pricing effectiveness.

**Building on**: The existing turnaround cost calculation from the base exercise remains unchanged. This exercise adds new features on top of that foundation.

**Important**: You must implement all calculations with **decimal precision**. Floating-point arithmetic will cause test failures.

---

## What You Already Have (Starter Code)

### Data Model (Already Provided)

Your starter code includes a ready-made data model with three entities:

#### 1. `Flight`
```csharp
public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; }  // e.g., "FR1234"
    public string Route { get; set; }         // e.g., "VIE-LPA"
    public DateTime FlightDate { get; set; }
    public int SeatCapacity { get; set; }     // 220 or 240
    public int SoldSeats { get; set; }
    public decimal BaseFare { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }    // From existing turnaround calculation
}
```

#### 2. `BookingRecord`
```csharp
public class BookingRecord
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public Flight Flight { get; set; }
    public DateTime BookingDate { get; set; }  // When the ticket was purchased
    public decimal TicketPrice { get; set; }
    public int PassengerCount { get; set; }    // Usually 1, but can be group bookings
}
```

#### 3. `Route`
```csharp
public class Route
{
    public int Id { get; set; }
    public string RouteCode { get; set; }      // e.g., "VIE-LPA"
    public string Origin { get; set; }         // e.g., "VIE"
    public string Destination { get; set; }    // e.g., "LPA"
    public int TypicalDemand { get; set; }     // Historical average PAX (0-300)
    public decimal AverageFare { get; set; }   // Historical average ticket price
}
```

### Database Context

- `ApplicationDataContext` is already configured with these entities
- Migrations are already created
- Test infrastructure uses in-memory SQLite (via `DatabaseFixture`)

### Basic Code Structure

You will find starter code for:
- ✅ Importer project with `FileReader` class
- ✅ CSV parser skeleton (`BookingDataCsvParser`)
- ✅ Database writer skeleton (`BookingImportDatabaseWriter`)
- ✅ Web API with basic endpoints (`FlightPlanningEndpoints`)
- ✅ Angular UI with routing and basic components
- ✅ Test projects with example test classes

---

## Your Tasks

### 1. Non-Trivial Business Logic (Backend)

You must implement **three** pieces of business logic in the `AppServices` project:

#### 1.1 Configuration Optimizer (`IConfigurationOptimizer`)

**Purpose**: Analyze historical booking data for a route and recommend whether to use a 220-seat or 240-seat A321neo configuration.

**Interface** (already provided):
```csharp
public interface IConfigurationOptimizer
{
    ConfigurationRecommendation RecommendConfiguration(string routeCode, List<BookingRecord> historicalBookings);
}

public class ConfigurationRecommendation
{
    public int RecommendedCapacity { get; set; }  // 220 or 240
    public decimal ExpectedLoadFactor { get; set; }  // 0.00 to 1.00
    public string Reasoning { get; set; }
}
```

**Business Rules**:
1. Calculate the **average sold seats** from historical bookings for this route
2. Calculate the **expected load factor** for each configuration:
   - Load Factor = Average Sold Seats / Seat Capacity
3. **Recommendation logic**:
   - If average sold seats ≤ 195: recommend **220** (load factor will be good, lower risk of empty seats)
   - If average sold seats > 195 and ≤ 220: recommend **220** (optimal load factor)
   - If average sold seats > 220 and ≤ 235: recommend **240** (can accommodate demand without major spillage)
   - If average sold seats > 235: recommend **240** and warn about potential spillage
4. **Edge cases**:
   - If no historical data exists, return 220 as conservative default
   - If historical data shows high variance (standard deviation > 25% of mean), mention this in reasoning
5. Reasoning must explain the recommendation (e.g., "Average demand of 232 PAX exceeds 220-seat capacity. 240-seat config achieves 96.7% load factor.")

#### 1.2 Schedule Validator (`IScheduleValidator`)

**Purpose**: Check if a proposed daily flight schedule is feasible given aircraft constraints.

**Interface** (already provided):
```csharp
public interface IScheduleValidator
{
    ScheduleValidationResult ValidateSchedule(List<FlightScheduleEntry> proposedFlights);
}

public class FlightScheduleEntry
{
    public string FlightNumber { get; set; }
    public TimeOnly DepartureTime { get; set; }
    public TimeOnly ArrivalTime { get; set; }  // Back at VIE
    public int FlightDurationMinutes { get; set; }
}

public class ScheduleValidationResult
{
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; }
}
```

**Business Rules**:
1. **Minimum turnaround time**: 60 minutes between arrival and next departure
2. **Night curfew**: No departures between 23:00 and 06:00 (Vienna Airport restriction)
3. **Flight order**: Flights must be chronologically ordered by departure time
4. **Same aircraft**: All flights use the same aircraft (sequential operation)
5. **Validation checks**:
   - Each flight's arrival time must be after its departure time
   - Turnaround time between consecutive flights ≥ 60 minutes
   - No departures during curfew
   - Total scheduled time must fit within 24 hours

**Example**:
```
Valid schedule:
- FR1234: Depart 07:00, Arrive 10:30 
- FR1235: Depart 12:00, Arrive 15:30 (90 min turnaround ✓)
- FR1236: Depart 17:00, Arrive 20:30 (90 min turnaround ✓)

Invalid schedule:
- FR1234: Depart 07:00, Arrive 10:30
- FR1235: Depart 11:00, Arrive 14:30 (only 30 min turnaround ✗)
```

#### 1.3 Booking Curve Analyzer (`IBookingCurveAnalyzer`)

**Purpose**: Analyze when tickets were purchased and determine pricing effectiveness.

**Interface** (already provided):
```csharp
public interface IBookingCurveAnalyzer
{
    BookingCurveAnalysis AnalyzeBookingPattern(int flightId, List<BookingRecord> bookings);
}

public class BookingCurveAnalysis
{
    public int TotalBookings { get; set; }
    public decimal EarlyBirdPercentage { get; set; }  // Booked >14 days before flight
    public decimal LastMinutePercentage { get; set; } // Booked ≤7 days before flight
    public decimal AverageTicketPrice { get; set; }
    public decimal RevenueOptimizationScore { get; set; }  // 0.00 to 1.00
    public string Analysis { get; set; }
}
```

**Business Rules**:
1. **Booking categories** (based on days before flight):
   - **Early Bird**: > 14 days before flight
   - **Standard**: 8-14 days before flight
   - **Last Minute**: ≤ 7 days before flight
2. **Calculate percentages** for each category
3. **Revenue optimization score**:
   - Compare actual revenue distribution to theoretical optimal distribution
   - Optimal distribution (from marketing research): 30% early bird, 45% standard, 25% last minute
   - Score = 1.0 - (sum of absolute differences / 2)
   - Example: If actual is 40% early, 40% standard, 20% last minute:
     - Differences: |40-30|=10, |40-45|=5, |20-25|=5, sum=20
     - Score = 1.0 - (20/200) = 0.90
4. **Analysis text**: describe if pricing tiers are attracting the right booking curve
   - Score > 0.85: "Well-optimized pricing"
   - Score 0.70-0.85: "Acceptable pricing distribution"
   - Score < 0.70: "Pricing may need adjustment - too many {category} bookings"

### 2. Non-Trivial Data Import

Extend the importer to handle booking timeline data from CSV files.

#### 2.1 CSV Format

**File**: `BookingData.csv`

```csv
FlightNumber,BookingDate,TicketPrice,PassengerCount,FlightDate
FR1234,2026-01-15,29.99,1,2026-02-01
FR1234,2026-01-20,39.99,2,2026-02-01
FR1234,2026-01-28,49.99,1,2026-02-01
FR1235,2026-02-01,29.99,1,2026-02-15
```

#### 2.2 Import Logic Requirements

You must extend `BookingDataCsvParser` and `BookingImportDatabaseWriter` to:

1. **Parse the CSV**:
   - Handle header row
   - Parse dates (ISO format: YYYY-MM-DD)
   - Parse decimal prices (format: "29.99")
   - Handle missing or malformed data

2. **Validation rules**:
   - `BookingDate` must be before `FlightDate`
   - `TicketPrice` must be > 0 and < 1000
   - `PassengerCount` must be ≥ 1 and ≤ 9 (max group size)
   - `FlightNumber` must match pattern: 2 letters + 4 digits (e.g., FR1234)

3. **Data quality handling**:
   - **Skip invalid rows** but log a warning (count discarded rows)
   - **Detect duplicates**: same FlightNumber + BookingDate + TicketPrice + PassengerCount
   - After import, return summary:
     ```csharp
     public class ImportSummary
     {
         public int TotalRowsProcessed { get; set; }
         public int SuccessfulImports { get; set; }
         public int SkippedRows { get; set; }
         public List<string> Warnings { get; set; }
     }
     ```

4. **Flight matching**:
   - For each booking record, find or create the corresponding `Flight` entity
   - If flight doesn't exist, create it with default values:
     - SeatCapacity = 220 (conservative default)
     - BaseFare = lowest ticket price seen for this flight
     - Route = "UNKNOWN" (to be filled manually later)

5. **Aggregation**:
   - After all bookings are imported, update each `Flight` entity:
     - `SoldSeats` = sum of all PassengerCount for that flight
     - `TotalRevenue` = sum of all (TicketPrice × PassengerCount) for that flight

#### 2.3 Command-Line Interface

The importer must accept:
```bash
dotnet run -- import-bookings --file path/to/BookingData.csv
```

Example output:
```
Processing BookingData.csv...
Total rows: 1247
Successful imports: 1198
Skipped (invalid): 49
Warnings:
  - Row 23: BookingDate after FlightDate (skipped)
  - Row 156: Invalid flight number format (skipped)
  - Row 487: Duplicate booking (skipped)
Import complete.
```

### 3. Web API Extension

Add new endpoints to `FlightPlanningEndpoints.cs`.

#### 3.1 Configuration Recommendation Endpoint

```csharp
// POST /api/configuration/recommend
// Request body:
{
  "routeCode": "VIE-LPA"
}

// Response (200 OK):
{
  "recommendedCapacity": 240,
  "expectedLoadFactor": 0.967,
  "reasoning": "Average demand of 232 PAX exceeds 220-seat capacity. 240-seat config achieves 96.7% load factor."
}
```

**Requirements**:
- Query historical bookings for the specified route
- Use `IConfigurationOptimizer` to get recommendation
- Return 404 if route doesn't exist
- Return 400 if routeCode is empty or invalid format

#### 3.2 Schedule Validation Endpoint

```csharp
// POST /api/schedule/validate
// Request body:
{
  "flights": [
    {
      "flightNumber": "FR1234",
      "departureTime": "07:00",
      "arrivalTime": "10:30",
      "flightDurationMinutes": 210
    },
    {
      "flightNumber": "FR1235",
      "departureTime": "12:00",
      "arrivalTime": "15:30",
      "flightDurationMinutes": 210
    }
  ]
}

// Response (200 OK):
{
  "isValid": true,
  "validationErrors": []
}

// Or if invalid (200 OK with errors):
{
  "isValid": false,
  "validationErrors": [
    "Insufficient turnaround time between FR1234 and FR1235 (only 30 minutes, minimum 60 required)"
  ]
}
```

#### 3.3 Booking Analysis Endpoint

```csharp
// GET /api/bookings/analyze/{flightId}

// Response (200 OK):
{
  "totalBookings": 187,
  "earlyBirdPercentage": 0.32,
  "lastMinutePercentage": 0.23,
  "averageTicketPrice": 67.45,
  "revenueOptimizationScore": 0.91,
  "analysis": "Well-optimized pricing with good distribution across booking periods."
}
```

**Requirements**:
- Return 404 if flight not found
- Return 204 if flight has no bookings

#### 3.4 Route Comparison Endpoint

```csharp
// GET /api/routes/compare

// Response (200 OK):
[
  {
    "routeCode": "VIE-LPA",
    "totalFlights": 156,
    "averageLoadFactor": 0.89,
    "averageRevenue": 14567.80,
    "averageCost": 8234.50,
    "averageProfit": 6333.30,
    "recommendedConfiguration": 220
  },
  {
    "routeCode": "VIE-AGP",
    "totalFlights": 143,
    "averageLoadFactor": 0.95,
    "averageRevenue": 17234.90,
    "averageCost": 8456.20,
    "averageProfit": 8778.70,
    "recommendedConfiguration": 240
  }
]
```

**Requirements**:
- Aggregate data from all flights grouped by route
- Calculate averages for revenue, cost, profit
- Use `IConfigurationOptimizer` for recommendation
- Sort by average profit (descending)

### 4. Angular Frontend Dashboard

Create a multi-tab dashboard with the following views:

#### 4.1 Route Comparison View (Primary Tab)

**Component**: `RouteComparisonComponent`

**Features**:
1. Display a table with all routes from the `/api/routes/compare` endpoint
2. Columns:
   - Route Code
   - Total Flights
   - Avg Load Factor (as percentage, e.g., "89%")
   - Avg Revenue (€, 2 decimals)
   - Avg Cost (€, 2 decimals)
   - Avg Profit (€, 2 decimals, color-coded: green if positive, red if negative)
   - Recommended Config (220 or 240)

3. **Client-side filtering**:
   - Filter input above table: filter by route code (substring match, case-insensitive)
   - Example: typing "LPA" shows only routes containing "LPA"

4. **Client-side sorting**:
   - Click column headers to sort ascending/descending
   - Default sort: by average profit (descending)
   - Visual indicator (↑/↓) for current sort column

5. **Summary row** at bottom:
   - Total flights across all (filtered) routes
   - Average load factor across all (filtered) routes
   - Total profit across all (filtered) routes

**Non-trivial logic**:
- Filtering and sorting must work together (sort filtered results)
- Summary row must recalculate when filter changes
- Load factor averaging: you must weight by number of flights (not simple average of averages)

#### 4.2 Configuration Optimizer View (Secondary Tab)

**Component**: `ConfigurationOptimizerComponent`

**Features**:
1. Input form:
   - Route code (text input with validation: 7 characters, format XXX-XXX)
   - Submit button

2. On submit:
   - Call `POST /api/configuration/recommend`
   - Display result:
     - "Recommended Configuration: 220 seats" (or 240)
     - Expected load factor (as percentage with 1 decimal: "96.7%")
     - Reasoning text

3. **Error handling**:
   - If route not found (404): show "No historical data for this route"
   - If invalid format: show validation error inline

4. **Comparison display** (non-trivial logic):
   - After getting recommendation, display a comparison table:
     ```
     Configuration | Expected Load Factor | Expected Spillage
     220 seats     | 105.5%*             | ~12 PAX
     240 seats     | 96.7%               | 0 PAX
     
     * Exceeds 100% - demand exceeds capacity
     ```
   - Calculate spillage: max(0, average demand - seat capacity)

#### 4.3 Booking Timeline View (Tertiary Tab)

**Component**: `BookingTimelineComponent`

**Features**:
1. Input: Flight ID (number input)
2. On submit: Call `GET /api/bookings/analyze/{flightId}`
3. Display results:
   - Total bookings
   - Booking distribution (visual bar chart using CSS):
     - Early Bird: X% (green bar)
     - Standard: Y% (blue bar)
     - Last Minute: Z% (orange bar)
   - Average ticket price
   - Revenue optimization score (as percentage with 1 decimal)
   - Analysis text

4. **Non-trivial logic**:
   - Calculate bar widths proportionally (using percentages)
   - Color-code optimization score:
     - ≥85%: green
     - 70-84%: yellow
     - <70%: red

**Navigation**:
- Use Angular routing
- Tab navigation bar at top
- Active tab highlighted

### 5. Automated Tests

You must extend/create tests in `AppServicesTests` and `WebApiTests`.

#### 5.1 Unit Tests for Business Logic

**File**: `AppServicesTests/ConfigurationOptimizerTests.cs`

Test cases to implement:
1. `RecommendConfiguration_LowDemand_Recommends220()`
   - Average demand: 180 PAX
   - Expected: 220 seats, load factor 0.818

2. `RecommendConfiguration_MediumDemand_Recommends220()`
   - Average demand: 210 PAX
   - Expected: 220 seats, load factor 0.955

3. `RecommendConfiguration_HighDemand_Recommends240()`
   - Average demand: 230 PAX
   - Expected: 240 seats, load factor 0.958

4. `RecommendConfiguration_NoHistoricalData_ReturnsDefault220()`
   - Empty booking list
   - Expected: 220 seats (conservative default)

5. `RecommendConfiguration_HighVariance_IncludesWarningInReasoning()`
   - Bookings with high standard deviation (>25% of mean)
   - Expected: reasoning contains "variance" or "inconsistent demand"

**File**: `AppServicesTests/ScheduleValidatorTests.cs`

Test cases to implement:
1. `ValidateSchedule_ValidSchedule_ReturnsTrue()`
2. `ValidateSchedule_InsufficientTurnaround_ReturnsFalse()`
3. `ValidateSchedule_CurfewViolation_ReturnsFalse()`
4. `ValidateSchedule_UnorderedFlights_ReturnsFalse()`
5. `ValidateSchedule_NegativeFlightDuration_ReturnsFalse()`

**File**: `AppServicesTests/BookingCurveAnalyzerTests.cs`

Test cases to implement:
1. `AnalyzeBookingPattern_OptimalDistribution_HighScore()`
   - 30% early, 45% standard, 25% last minute
   - Expected score: 1.00 (perfect)

2. `AnalyzeBookingPattern_SkewedDistribution_LowerScore()`
   - 60% early, 30% standard, 10% last minute
   - Expected score: < 0.85

3. `AnalyzeBookingPattern_CalculatesAverageTicketPrice()`
   - Various ticket prices
   - Verify weighted average is correct

#### 5.2 Integration Tests for Web API

**File**: `WebApiTests/FlightPlanningEndpointsTests.cs`

Test cases to implement:
1. `ConfigurationRecommend_ValidRoute_Returns200()`
2. `ConfigurationRecommend_InvalidRoute_Returns404()`
3. `ScheduleValidate_ValidSchedule_Returns200WithIsValidTrue()`
4. `ScheduleValidate_InvalidSchedule_Returns200WithErrors()`
5. `BookingAnalyze_ExistingFlight_Returns200()`
6. `BookingAnalyze_NonexistentFlight_Returns404()`
7. `RoutesCompare_ReturnsAllRoutesSortedByProfit()`

**Test data setup**:
- Use `DatabaseFixture` for in-memory database
- Seed test data in test constructor
- Clean up after each test

### 6. Documentation

Update `starter/README.md` to include:

1. **Business Logic Overview**: Brief description of the three business logic components
2. **API Endpoints**: Document all new endpoints with request/response examples
3. **Running the Importer**: Command-line examples
4. **Testing Instructions**: How to run the tests

---

## Grading Criteria

| Category | Points | Description |
|----------|--------|-------------|
| **Business Logic** | 30 | All three components implemented correctly with edge case handling |
| **Data Import** | 15 | CSV parsing, validation, error handling, aggregation |
| **Web API** | 15 | All endpoints working, proper status codes, error handling |
| **Angular Frontend** | 25 | All three views functional, client-side logic correct, good UX |
| **Tests** | 10 | All tests passing, good coverage |
| **Code Quality** | 5 | Clean code, no magic numbers, proper decimal handling |

**Total: 100 points**

---

## Tips for Success

### Decimal Precision
```csharp
// Correct
decimal price = 29.99m;  // note the 'm' suffix
decimal total = price * quantity;

// Wrong (will cause rounding errors)
double price = 29.99;
```

### Testing with Decimal
```csharp
// Correct
Assert.Equal(0.955m, loadFactor, 3);  // compare with 3 decimal places

// Wrong
Assert.Equal(0.955, loadFactor);  // type mismatch
```

### Date Calculations
```csharp
// Days between booking and flight
var daysBeforeFlight = (flight.FlightDate - booking.BookingDate).TotalDays;

if (daysBeforeFlight > 14)
{
    // Early bird
}
```

### Angular Signal Updates
```typescript
// When filter changes, update filtered results
filterText.set(value);
// Computed signal will automatically recalculate:
filteredRoutes = computed(() => {
  const filter = this.filterText();
  return this.routes().filter(r => 
    r.routeCode.toLowerCase().includes(filter.toLowerCase())
  );
});
```

### Error Handling in Import
```csharp
try
{
    var bookingDate = DateTime.Parse(row[1]);
    if (bookingDate >= flightDate)
    {
        summary.SkippedRows++;
        summary.Warnings.Add($"Row {rowNumber}: BookingDate after FlightDate");
        continue;
    }
}
catch (FormatException)
{
    summary.SkippedRows++;
    summary.Warnings.Add($"Row {rowNumber}: Invalid date format");
    continue;
}
```

---

## Questions?

If any requirement is unclear, check:
1. The starter code interfaces (they define exact signatures)
2. The test cases (they show expected behavior)
3. The API endpoint examples (they show exact JSON format)

Good luck! 