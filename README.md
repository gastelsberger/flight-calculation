# flight-calculation

## Einleitung

Diese Aufgabenstellung lässt uns das rechnen, was im ULCC-Alltag wirklich zählt. Wir bauen ein Programm, das für einen Turnaround (Ankunft + Standzeit + Abflug) am Flughafen Wien (VIE) die airport-bezogenen Kosten aus offiziellen Entgeltparametern berechnet und parallel eine Ticket-Preisstaffelung simuliert (erste 30% günstig, danach je 15% +12,5% Preis). Die Gebührenwerte stammen aus der Entgeltordnung 2026 (gültig ab 01.01.2026, netto ohne USt) und werden in dieser Aufgabenstellung als Konstanten geführt, damit wir Updates sauber einpflegen können. 

Wichtig: Die Sitzplatzkapazität des A321neo ist hier absichtlich unbestimmt. Wir implementieren und testen zwei gängige Optionen (ULCC 220 oder 240) als Parameter. 

Für den Realitätscheck: Airbus nennt für den A321neo MTOW 97t, typische Bestuhlung 180–220, maximale Bestuhlung 244.

Ein ULCC betreibt einen Umlauf am Flughafen Wien. Das Flugzeug ist ein A321neo mit MTOW 97t (gegeben) und wird typischerweise auf einer offenen Vorfeldposition abgefertigt (Busboarding statt Pier, weil Kosten). Für den Abflug werden lokal abfliegende Passagiere angenommen (keine Transfer-PAX), um das Modell für den Einstieg beherrschbar zu halten. 

## Begriffe:

- MTOW (Maximum Take-Off Weight): Bemessungsgrundlage für das Landeentgelt; in der Entgeltordnung wird MTOW auf die nächsten vollen 1.000 kg gerundet (praktisch: auf volle Tonnen nach oben). 
- BLOCK-ON / BLOCK-OFF: Start/Ende der Standzeit am Abstellplatz; Parkentgelt basiert auf der Differenz (Blockzeit). 
Turnaround (für diese Aufgabe): 1× Landung in VIE, Standzeit (Parken), 1× Start in VIE; deshalb fallen manche Entgelte pro Landung und pro Start an. 
- Positionierung (Vorfeld vs Pier): Infrastrukturentgelt „Rampe“ ist abhängig von Pierposition oder offener Vorfeldposition und vom LFZ-Typ (Gruppenzuordnung). 
- PAX-Entgelte: Entgelte pro abfliegendem Passagier (z. B. Fluggastentgelt, Infrastrukturentgelt „Passage“, PRM-Entgelt, Sicherheitsentgelt).

Scope-Entscheidung:
Die Entgeltordnung enthält zusätzlich u. a. Lärmentgelt (separates Dokument) und Betankungs-Infrastrukturentgelt. Für diese Aufgabe fokussieren wir auf die geforderten Kernparameter (Landung, Parken, Position, PAX-Entgelte). 

## Formeln und Preistabelle

| Gebührenblock | Parameter | Einheit | Wert | Gilt wann / Bemessung |
|---|---:|---:|---:|---|
| **Landeentgelt (Passagierflug)** | Fixer Entgelt-Teil | €/Landung | 269,57 | Immer für Passagierflüge |
|  | Variabler Entgelt-Teil | €/t MTOW | 0,00 bis 45 t; 7,36 ab 46 t | Variable Komponente wird bei MTOW >45 t angewandt und bezieht sich auf das **Gesamt-MTOW** (nicht nur „über 45 t“) |
| **Parkentgelt** | Freiparkzeit | h | 4,0 | Bis 4h: 0 € |
|  | Parkentgelt nach Freiparkzeit | % des Landeentgelts | 15% | Pro **angefangene 24h** nach Ablauf der 4h-Freiparkzeit |
|  | Nachtruhe-Regel | Zeitfenster | 22:00–06:00 | In diesem Fenster kein Parkentgelt; angrenzende Zeiten werden addiert. (Bonus-Teilaufgabe) |
| **Infrastrukturentgelt „Rampe“** | Gruppe 3 (A320/A321/…) – Pierposition | €/Movement | 176,17 | Pro Landung **und** pro Start, abhängig von Position |
|  | Gruppe 3 – Offene Vorfeldposition | €/Movement | 122,71 | Für ULCC-Default in dieser Aufgabe |
| **Fluggastentgelt** | Lokal abfliegender Fluggast | €/PAX | 20,51 | Pro abfliegendem lokalen Fluggast |
| **Infrastrukturentgelt „Passage“** | Abfliegender Fluggast | €/PAX | 1,08 | Wird in Verbindung mit Fluggastentgelt erhoben |
| **PRM-Entgelt** | Basissatz | €/PAX | 0,86 | Pro abfliegendem Fluggast (Zuschläge optional) |
| **Sicherheitsentgelt** | Lokal abfliegend (und Transfer) | €/PAX | 10,52 | Pro abfliegendem Fluggast |

### MTOW-Rundung:
Eingabe kann in t oder kg erfolgen (entscheidet ihr, aber dokumentiert es).
Regel: auf die nächsten vollen 1.000 kg runden (Beispiel in der Entgeltordnung). 
Landeentgelt (Passagierflug, Linien-/Charterverkehr):

Wenn mtow_t <= 45:
- landing_fee = 269.57 
Wenn mtow_t >= 46:
- landing_fee = 269.57 + (mtow_t * 7.36) 

### Parkentgelt:
Freiparkzeit free_hours = 4. 
Danach: pro angefangene 24 Stunden: 0.15 * landing_fee. 
Minimalmodell (Pflichtteil): Nutzt parkdauer_h als gesamte Blockzeit und ignoriert 22:00–06:00.
Erweiterung (Bonus): Abzug der Stunden im Fenster 22:00–06:00 gemäß Regel. 
Infrastrukturentgelt „Rampe“ (Gruppe 3 für A321):

ramp_fee_per_movement = 122.71 für offene Vorfeldposition, 176.17 für Pierposition. 
Für Turnaround: ramp_fee_total = ramp_fee_per_movement * 2 (Landung + Start). 
PAX-Entgelte (lokal abfliegend):
pax_fee_per_pax = 20.51 + 1.08 + 0.86 + 10.52 = 32.97 €/PAX. 

Total Airport Cost (Turnaround):
total_cost = landing_fee + parking_fee + ramp_fee_total + (pax_count * pax_fee_per_pax)

## Funktionale Anforderungen
Unser Programm muss mindestens folgende Eingaben akzeptieren als JSON-Datei:

- mtow_t oder mtow_kg (MTOW als Zahl, >0) – Default: 97 t. 
- seat_capacity (Ganzzahl) – erlaubt: 220 oder 240 (zwei vorkonfigurierte Optionen). Kontext: A321neo kann bis 244; typisch 180–220; ULCC-Lösungen sind oft hoch verdichtet. 
- pax_count (Ganzzahl, 0…seat_capacity)
- parking_duration_h (Zahl, ≥0, Blockzeit am Stand; Freiparkzeit wird intern angewandt) 
- stand_position (Enum): "VORFELD" oder "PIER" (Default: VORFELD) 
- base_fare (Zahl >0): Ticketpreis für die erste Preisstufe (30%)
- Optional (Bonus): parking_night_free_h oder echte Zeitstempel block_on, block_off, um 22:00–06:00 korrekt zu berücksichtigen. 

### Preisstaffelung
Implementiere eine Ticketpreis-Logik:

- Tier 1: Erste 30% der Sitzplätze zum base_fare.
- Danach: je 15% der Sitzplätze als nächster Tier, Preis steigt je Tier um +12,5% (Multiplikator 1.125).
Fortsetzen, bis alle seat_capacity Plätze einem Tier zugeordnet sind. (Letzter Tier ist der Rest, falls nicht exakt aufgeht.)

**Tipp:** ceil für die Tier-Sitzanzahl von 30% und 15% und weist den Rest dem letzten Tier zu, damit ihr exakt auf seat_capacity kommt.
Passagiere werden von billig nach teuer aufgefüllt, bis pax_count erreicht ist.

### Report

Das Programm muss einen **klar formatierten Report** im Frontend anzeigen.  

---

### Pflichtfelder im Report

#### 1) Eingabeparameter
Muss enthalten:

- **mtow_input_t** (z. B. 97.0)
- **mtow_rounded_t** (gerundet nach Vorgabe)
- **seat_capacity** (nur 220 oder 240 erlaubt)
- **pax_count**
- **parking_duration_hours**
- **position** (z. B. `"remote_stand"` / `"apron"` / `"pier"` – je nach eurem Modell)

> Wichtig: **Alles was in den Report kommt, muss exakt die effektiven Werte zeigen**, also inkl. Rundung und Normalisierung.

---

#### 2) Einzelkosten (netto, ohne USt.)
Muss als Posten enthalten:

- **landing_fee**
- **parking_fee**
- **ramp_fee**
- **passenger_fees**

Format:
- Geldwerte **kaufmännisch auf 2 Dezimalstellen**
- immer **EUR**
- **netto ohne USt.**

---

#### 3) Summe
- **total_cost**

---

#### 4) Preisstaffelung (Tiers)
Als Tabelle mit exakt diesen Spalten:

- **tier_index**
- **seats_in_tier**
- **sold_seats_in_tier**
- **price**
- **revenue**

Regel (muss 1:1 umgesetzt sein):
- **erste 30%** der Sitze: billigster Preis
- danach **je 15% Tier**
- jedes Tier: **+12,5%** gegenüber dem vorherigen Tier
- die Tiergrenzen beziehen sich auf **seat_capacity**
- Umsatz = sold_seats_in_tier × price

---

#### 5) Revenue & Ergebnis
- **total_revenue**
- **profit_loss** (= total_revenue − total_cost)

---

## Formatvorgabe

- **EUR**
- **2 Dezimalstellen**, **kaufmännisch** (round half up / bankers rounding ist NICHT “kaufmännisch”)
- **netto ohne USt.**
- Keine Floating-Point-Rundungsfehler im Output (wenn da `12.0000000004` steht: direkt durchgefallen)

## Qualitätsanforderungen

### Reproduzierbarkeit
- Gebührenwerte stehen **zentral** (z. B. `fees.ts` / `fees.json`)
- **Keine Magic Numbers** im Code
- Werte müssen **exakt** den vorgegebenen Konstanten entsprechen

### Genauigkeit
- Geldbeträge mit **Decimal-Logik** rechnen  
- Floating-Point ist nicht “fast richtig”, sondern **falsch**.

### Fehlerbehandlung (harte Errors)
- `pax_count < 0` oder `pax_count > seat_capacity` → **klarer Fehler**
- `seat_capacity` nicht **220/240** → **klarer Fehler**
- negative Zeiten/Preise → **klarer Fehler**

### Testbarkeit
- kleine Funktionen (pure functions)
- I/O getrennt halten (Parsing/CLI getrennt von Berechnung)