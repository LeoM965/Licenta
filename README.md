# AgroBot Simulator — Platformă de Simulare pentru Roboți Agricoli Autonomi

Proiect de licență ce implementează o platformă 3D interactivă de simulare a unei ferme automatizate, în care roboți autonomi gestionează ciclul complet de viață al culturilor agricole: de la analiza solului, plantare și irigare, până la recoltare și calculul economic al profitabilității.

Aplicația este dezvoltată în **Unity 2022+** folosind **C#** și **PolygonFarm** ca pachet de asset-uri 3D.

---

## Cuprins

- [Prezentare Generală](#prezentare-generală)
- [Arhitectura Proiectului](#arhitectura-proiectului)
- [Modulul AI — Inteligență Artificială](#modulul-ai--inteligență-artificială)
- [Modulul Roboți](#modulul-roboți)
- [Modulul Culturi](#modulul-culturi)
- [Modulul Senzori de Sol](#modulul-senzori-de-sol)
- [Modulul Economic](#modulul-economic)
- [Modulul Meteo](#modulul-meteo)
- [Interfața Utilizator (UI)](#interfața-utilizator-ui)
- [Modulul Cameră](#modulul-cameră)
- [Spawners și Generare Procedurală](#spawners-și-generare-procedurală)
- [Manageri Globali](#manageri-globali)
- [Date Agronomice și Surse](#date-agronomice-și-surse)
- [Cerințe Sistem](#cerințe-sistem)
- [Cum se Rulează](#cum-se-rulează)

---

## Prezentare Generală

Simularea rulează pe o fermă virtuală cu **200 de parcele agricole** distribuite în **4 zone**, pe care operează **3 roboți autonomi** cu roluri specializate. Fiecare parcelă conține un senzor de mediu care analizează compoziția solului în timp real (pH, azot, fosfor, potasiu, umiditate). Roboții utilizează algoritmul **A\* (A-star)** pentru navigare, un sistem de **task-uri prioritare** pentru luarea deciziilor, și un model economic complet pentru evaluarea profitabilității.

Simularea include un ciclu zi-noapte, un sistem meteo cu 5 tipuri de vreme (însorit, noros, ploaie, furtună, ceață), profiluri climatice sezoniere, și o interfață grafică completă cu dashboard, minimapă, și panouri de informații detaliate.

---

## Arhitectura Proiectului

```
Assets/Scripts/
├── AI/                     # Inteligență artificială
│   ├── Analytics/          # Tracking decizii (DecisionTracker)
│   ├── Core/               # Task manager, scanners (Harvest, Soil)
│   ├── DataStructures/     # MinHeap pentru A*
│   ├── Models/             # Modele decizii și task-uri
│   └── Navigation/         # Pathfinding A*, PathGrid, PathNode
├── Camera/                 # Sistem cameră multi-mod
│   ├── Core/               # RobotCamera (follow, orbit, free)
│   ├── Models/             # CameraMode, CameraSettings, CameraState
│   ├── Services/           # CameraDisplay (HUD info)
│   └── Utilities/          # CameraHelper
├── Crops/                  # Sistem culturi agricole
│   ├── Components/         # CropGrowth, CropSelector, CropHarvestVisuals
│   ├── Models/             # CropData, CropRequirements, CropRange, CropStage
│   └── Services/           # CropDatabase, CropLoader, CropManager, CropPool
├── Economics/              # Sistem economic
│   ├── Managers/           # RobotEconomicsManager, EconomicsHistoryManager
│   ├── Models/             # EconomicModels (statistici, istoric)
│   └── Services/           # CropEconomicsCalculator
├── Managers/               # Manageri globali
│   ├── TimeManager.cs      # Ciclul zi-noapte, calibrare timp
│   ├── SimulationSpeedController.cs
│   └── SimulationSpeedUI.cs
├── Robots/                 # Sistem roboți autonomi
│   ├── Base/               # RobotOperator (clasă abstractă)
│   ├── Capabilities/       # Flight, Harvesting, Planting (logică + modele)
│   ├── Core/               # BoundsHelper, RobotHelper, RobotLifecycle
│   ├── Data/               # OperationRegion, RobotDataLoader
│   ├── Energy/             # RobotEnergy, RobotBattery, RobotStats
│   └── Movement/           # RobotMotor, RobotMovement, RobotPathfinder, WheelController
├── Sensors/                # Sistem senzori de mediu
│   ├── Components/         # EnvironmentalSensor, SensorVisuals, TerrainAnalyzer
│   ├── Helpers/            # ParcelHelper, TerrainHelper, ZoneHelper
│   ├── Models/             # SoilComposition, SoilAnalysis, SoilSettings
│   └── Services/           # SoilAnalysisService, SoilCompositionGenerator, ParcelCache
├── Settings/               # Meniu setări (SettingsMenu)
├── Spawners/               # Generare procedurală
│   ├── Core/               # SpawnConfig, SpawnPositionFinder, SpawnValidator
│   ├── Crops/              # ParcelSpawner
│   ├── Environment/        # TreeSpawner, FenceGenerator, BuildingSpawner, IrrigationSpawner
│   ├── Helpers/            # SpawnHelper
│   ├── Models/             # Building
│   └── Robots/             # MultiRobotSpawner
├── UI/                     # Interfață grafică
│   ├── Dashboard/          # FarmDashboard, SimulationUI
│   ├── Maps/               # MiniMap, MiniMapRenderer, ParcelNavigator, MapColors
│   ├── Menus/              # PauseMenu, tab-uri dashboard (Crop, Robot, History)
│   ├── Panels/             # CropStatsPanel, DecisionPanel, ParcelInfoPanel, RobotInfoPanel
│   ├── Styles/             # UITheme (sistem de stiluri global)
│   └── Utils/              # BatteryBarUI, GUITools, TimeHelper, UIDrawUtils
└── Weather/                # Sistem meteorologic
    ├── Components/         # WeatherSystem, WeatherSoilUpdater, RainAreaController
    ├── Editor/             # SeasonAssetGenerator
    ├── Models/             # ClimateProfile, WeatherProfile, WeatherImpact, WeatherType
    └── Services/           # WeatherSimulator, AtmosphereRenderer, PrecipitationManager, SoilMoistureService
```

**Total: ~130 de scripturi C#** organizate în 12 module.

---

## Modulul AI — Inteligență Artificială

### Navigare — Algoritmul A*

Navigarea utilizează un grid de tip **250×599** generat automat din datele terenului. Algoritmul A* (implementat în `Pathfinder.cs`) folosește o structură de date `MinHeap` pentru performanță optimă. Fiecare nod din grid (`PathNode`) conține informații despre traversabilitate, cost g/h/f, și vecinii adiacenți.

- **PathGrid** — Construiește gridul de navigare la pornirea scenei, analizând terenul și obstacolele.
- **Pathfinder** — Implementare A* cu suport pentru recalculare dinamică a rutei.
- **RobotPathfinder** — Componentă per robot care gestionează mișcarea pe traseu, detectarea blocajelor, și reroutarea.

### Sistem de Task-uri

Roboții utilizează un sistem de task-uri prioritare, scanate periodic:

| Task | Descriere |
|------|-----------|
| `HarvestTask` | Recoltare culturi mature |
| `ScoutTask` | Scanare și analiză sol |
| `IrrigationTask` | Irigare parcele uscate |
| `FertilizationTask` | Fertilizare sol sărac |
| `LimingTask` | Corectare pH acid |

Scannerele (`HarvestScanner`, `SoilScanner`) evaluează parcele și generează task-uri ordonate după scor. `TaskManager` coordonează asignarea task-urilor către roboți.

### Tracking Decizii

`DecisionTracker` înregistrează fiecare decizie a fiecărui robot: scorul ales, alternativele evaluate, și factorii de decizie (pH, azot, fosfor, potasiu, umiditate). Acest istoric este afișat în `DecisionPanel` din UI.

---

## Modulul Roboți

### Arhitectura

Toți roboții extind clasa abstractă `RobotOperator`, care implementează mașina de stări:

```
Idle → MovingToParcel → Working → Idle (sau Charging)
```

### Cei 3 Roboți

| Robot | Rol | Capabilitate Specială |
|-------|-----|----------------------|
| **AgBot** | Plantare | Selectează automat cultura optimă pe baza analizei solului |
| **AgroBot Hybrid** | Scanare + plantare | Zbor autonom, scanare aeriană a parcelelor |
| **HarvestBot** | Recoltare | Recoltează culturile mature și calculează greutatea recoltei |

### Sistem Energetic

Fiecare robot are o baterie (`RobotBattery`) cu capacitate limitată. `RobotEnergyManager` monitorizează consumul, estimează dacă energia este suficientă pentru misiunea curentă, și trimite robotul la stația de încărcare când este necesar.

- Consumul energetic depinde de distanța parcursă și durata operațiunii
- Încărcarea se face automat la stație, cu rata configurabilă
- Indicatorul vizual de baterie este afișat deasupra robotului (`BatteryBarUI`)

### Mișcare și Fizică

- `RobotMotor` — Controlează viteza, accelerația și rotația pe teren
- `RobotMovement` — Integrează pathfinding-ul cu motorul fizic
- `RobotWheelController` — Animație roți sincronizată cu mișcarea

---

## Modulul Culturi

### 7 Culturi cu Date Agronomice Reale

Datele sunt stocate în `CropData.json` cu surse verificate (USDA, FAO, Eurostat 2024):

| Cultură | Semință (EUR) | Recoltă (kg) | Preț (EUR/kg) | Zile Creștere | pH Optim |
|---------|--------------|--------------|---------------|--------------|----------|
| Porumb | 0.004 | 0.23 | 0.25 | 90 | 6.5 |
| Grâu | 0.001 | 0.003 | 0.22 | 120 | 6.5 |
| Roșie | 0.25 | 8.0 | 1.20 | 70 | 6.3 |
| Cartof | 0.06 | 1.2 | 0.45 | 100 | 5.8 |
| Fasole | 0.02 | 0.25 | 2.50 | 55 | 6.5 |
| Salată | 0.02 | 0.40 | 1.50 | 45 | 6.5 |
| Floarea Soarelui | 0.003 | 0.06 | 0.56 | 85 | 6.8 |

### Ciclul de Viață

Fiecare cultură trece prin 4 stadii de creștere (`CropStage`), vizualizate prin scalare progresivă (`CropVisualScaling`). La maturitate, recolta devine disponibilă pentru harvesting. `CropGrowth` gestionează tranziția între stadii pe baza timpului simulat.

### Selecție Inteligentă

`CropSelector.SelectBestCrop()` evaluează fiecare cultură din baza de date pe baza compatibilității cu compoziția solului din parcela curentă. Se calculează un **scor de potrivire** per cultură folosind `CropRequirements.CalculateTotalScore()`, iar cultura cu scorul maxim este aleasă automat.

---

## Modulul Senzori de Sol

### Înregistrare Sol

Fiecare parcelă conține un `EnvironmentalSensor` care monitorizeză 5 parametri cheie:

- **pH** — Aciditatea solului (interval 0–14)
- **Azot (N)** — Conținut de nitrogen (ppm)
- **Fosfor (P)** — Conținut de fosfor (ppm)
- **Potasiu (K)** — Conținut de potasiu (ppm)
- **Umiditate** — Conținutul de apă din sol (%)

### Generare Procedurală

`SoilCompositionGenerator` creează compoziții de sol realiste folosind algoritmi de zgomot Perlin, asigurând variație naturală între parcele. Sunt definite mai multe tipuri de sol (`AgroSoilType`) cu intervale specifice.

### Analiză

`SoilAnalysisService` calculează un **scor de calitate** (0–100%) prin evaluarea fiecărui parametru față de intervalele optime ale culturilor. Acest scor este utilizat de sistemul economic pentru a multiplica randamentul.

---

## Modulul Economic

### Calculatorul de Profit

`CropEconomicsCalculator` agregă datele financiare per parcelă:

```
Profit = (Greutate × Preț Piață × Multiplicator Calitate Sol) - Cost Semințe
```

Multiplicatorul de calitate (0–1) reflectă cât de apropiate sunt condițiile solului de cerințele optime ale culturii.

### Istorie Economică

`EconomicsHistoryManager` înregistrează periodic starea financiară a fermei, permițând vizualizarea evoluției profitului în timp prin tab-ul `HistoryDashboardTab`.

### Monitorizare Per Robot

`RobotEconomicsManager` urmărește contribuția fiecărui robot la economia fermei: câte parcele a procesat, ce culturi a plantat, și ce venit a generat.

---

## Modulul Meteo

### Simulare Meteorologică

`WeatherSimulator` gestionează 5 tipuri de vreme:

| Tip | Efect |
|-----|-------|
| Însorit | Evaporare crescută, temperatură ridicată |
| Noros | Efect neutru |
| Ploaie | Umiditate sol crescută, temperatură scăzută |
| Furtună | Ploaie intensă, posibil daune |
| Ceață | Vizibilitate redusă |

### Climat Sezonier

Profilurile climatice (`ClimateProfile`) definesc probabilități diferite pentru fiecare tip de vreme în funcție de sezon (primăvară, vară, toamnă, iarnă). Temperatura urmează un ciclu diurn realist, cu minime noaptea și maxime la amiază.

### Impact asupra Solului

`WeatherSoilUpdater` și `SoilMoistureService` actualizează umiditatea solului în funcție de precipitații. Zona de ploaie (`RainAreaController`) afectează doar parcelele din raza sa.

### Randare Atmosferică

`AtmosphereRenderer` și `PrecipitationManager` controlează efectele vizuale: particule de ploaie, iluminare ambiantă, și tranziții de atmosferă.

---

## Interfața Utilizator (UI)

### Dashboard Fermă (F1)

Afișează în timp real: număr parcele, roboți activi, calitate medie sol, pH mediu, umiditate, și distribuția parcelor (bune/slabe/critice).

### Minimapă

`MiniMap` și `MiniMapRenderer` generează o mini-hartă a fermei cu cod de culori pe baza calității solului. `ParcelNavigator` permite click pe minimapă pentru navigare directă la parcele.

### Panouri Informative

| Panou | Conținut |
|-------|---------|
| `RobotInfoPanel` | Stare, baterie, status curent, parcele procesate |
| `ParcelInfoPanel` | Compoziție sol, scor calitate, cultură activă |
| `CropStatsPanel` | Statistici economice per cultură |
| `DecisionPanel` | Ultima decizie AI, scor, alternative evaluate |

### Tab-uri Dashboard

- **Crop Tab** — Statistici detaliate per cultură (profit, recoltă, cost)
- **Robot Tab** — Performanța fiecărui robot
- **History Tab** — Grafic evoluție profit în timp

### Meniu Pauză

`PauseMenu` oferă acces la setări de simulare (viteză, cultură forțată, parametri senzori).

---

## Modulul Cameră

`RobotCamera` suportă 3 moduri de vizualizare:

| Mod | Descriere |
|-----|-----------|
| **Follow** | Urmărește robotul selectat din spate |
| **Orbit** | Cameră orbitală în jurul robotului |
| **Free** | Mișcare liberă pe hartă (WASD + mouse) |

`CameraDisplay` suprapune informații HUD despre robotul urmărit.

---

## Spawners și Generare Procedurală

| Spawner | Funcție |
|---------|--------|
| `ParcelSpawner` | Generează 200 de parcele în 4 zone |
| `MultiRobotSpawner` | Instanțiază și poziționează toți roboții |
| `TreeSpawner` | Populează harta cu vegetație, evitând clădirile |
| `FenceGenerator` | Creează garduri în jurul zonelor agricole |
| `BuildingSpawner` | Poziționează clădirile fermei |
| `IrrigationSpawner` | Plasează sisteme de irigare |

Toate spawner-ele folosesc `SpawnPositionFinder` și `SpawnValidator` pentru a evita suprapunerile și a respecta restricțiile de teren.

---

## Manageri Globali

- **TimeManager** — Gestionează ciclul zi-noapte cu calibrare distanță-timp (1 metru = 0.72 secunde simulate)
- **SimulationSpeedController** — Permite accelerarea/decelerarea simulării (1x, 2x, 5x, 10x)

---

## Date Agronomice și Surse

Datele utilizate pentru definirea culturilor provin din surse academice și statistice oficiale:

- **USDA** — United States Department of Agriculture (referință nutrienți sol)
- **FAO** — Food and Agriculture Organization (intervale optime culturi)
- **Eurostat** — European Statistical Office (prețuri piață agricolă 2024)
- **Cornell University** — Cooperative Extension (testare și interpretare nutrienți sol)
- **Ohio State University** — Extension (managementul fertilității solului)
- **Piețe românești** — romania-insider.com, ukragroconsult.com, tridge.com (prețuri piață RO 2024)

---

## Cerințe Sistem

| Cerință | Specificație |
|---------|-------------|
| Motor | Unity 2022.3 LTS sau mai nou |
| Platformă | Windows 10/11 (x64) |
| .NET | .NET Standard 2.1 |
| RAM | 8 GB minim |
| GPU | Compatibil DirectX 11 |
| Asset Pack | PolygonFarm (Synty Studios) |

---

## Cum se Rulează

1. Deschideți proiectul în Unity Hub
2. Selectați scena principală din `Assets/Scenes/`
3. Apăsați **Play** (sau Ctrl+P)
4. Folosiți tastele:
   - **F1** — Toggle Dashboard Fermă
   - **ESC** — Meniu Pauză
   - **Tab** — Schimbă robot urmărit
   - **1/2/3** — Mod cameră (Follow/Orbit/Free)
   - **+/-** — Viteză simulare

---

Proiect realizat ca lucrare de licență, demonstrând competențe avansate de programare orientată pe obiecte, inteligență artificială, simulare de procese agricole, și dezvoltare de aplicații interactive 3D.
