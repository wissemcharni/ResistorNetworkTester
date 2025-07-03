# Resistance Network Tester - Technical Documentation

## Overview

The Resistance Network Tester is a C# WinForms application designed to test the functionality of a resistance network circuit by applying different voltage combinations and measuring the resulting output voltage. The application simulates the testing process using virtual hardware interfaces.


## Technical Requirements

### Runtime Requirements
- .NET Framework 4.7.2 or higher / .NET 6.0+
- Windows operating system
- ~10MB disk space

### Development Requirements  
- Visual Studio 2019/2022
- C# 8.0+ language features
- Windows Forms development tools

## System Architecture

### Project Structure

```
ResistanceNetworkTester/
├── Program.cs                    # Application entry point
├── UI/
│   └── MainForm.cs              # Main user interface
├── Services/
│   └── ResistanceNetworkTesterService.cs  # Core testing logic
├── Models/
│   ├── ResistorNetwork.cs       # Circuit calculation model
│   ├── TestConfiguration.cs     # Test parameter definition
│   └── TestResult.cs            # Test result container
└── Hardware/
    ├── IPowerSupplyDc.cs        # Power supply interface
    ├── PowerSupply.cs           # Power supply implementation
    ├── IMultiMeter.cs           # Multimeter interface
    └── DigitalMultiMeter.cs     # Multimeter implementation
```

### Design Patterns Used
- **Service Layer Pattern**: Business logic separated in `ResistanceNetworkTesterService`
- **Repository Pattern**: Hardware abstractions through interfaces
- **Observer Pattern**: Event-driven communication between service and UI

## Core Components

### 1. ResistanceNetworkTesterService
**Purpose**: Orchestrates the entire testing process

**Key Responsibilities**:
- Manages hardware components (3 power supplies + 1 multimeter)
- Generates test configurations for all input combinations
- Executes tests asynchronously with cancellation support
- Provides progress feedback and results

**Key Methods**:
```csharp
public async Task StartTestAsync()           // Starts the complete test sequence
public void CancelTest()                     // Cancels running tests
private async Task<TestResult> RunSingleTestAsync()  // Executes one test case
```

**Events**:
- `TestResultAvailable`: Fired when a single test completes
- `StatusChanged`: Fired when test status updates

### 2. ResistorNetwork
**Purpose**: Calculates expected output voltage based on circuit physics

**Circuit Parameters**:
- R_F1: 12,000Ω
- R_F2: 24,000Ω  
- R_F3: 47,000Ω
- R_Pullup: 4,700Ω
- Vcc: 3.3V

### Circuit Parameters
| Parameter       | Value  | Description               |
|-----------------|--------|---------------------------|
| Pull-up Resistor| 4.7 kΩ | Connected to 3.3V source  |
| R_F1            | 12 kΩ  | Fuse 1 path resistance    |
| R_F2            | 24 kΩ  | Fuse 2 path resistance    |
| R_F3            | 47 kΩ  | Fuse 3 path resistance    |
| Vcc             | 3.3V   | Supply voltage            |

**Calculation Method**:
Uses parallel resistance network analysis:
```
Vout = (Vcc/R_Pullup + V1/R_F1 + V2/R_F2 + V3/R_F3) / (1/R_Pullup + 1/R_F1 + 1/R_F2 + 1/R_F3)
```

### 3. TestConfiguration
**Purpose**: Defines parameters for each test case

**Properties**:
- `TestName`: Unique identifier (Test_01 to Test_08)
- `F1_State`, `F2_State`, `F3_State`: Input voltage states (0V or 24V)
- `ExpectedVoltage`: Calculated expected output
- `TolerancePercent`: Acceptable deviation (5%)
- `AbsoluteTolerance`: Minimum tolerance (0.01V)

**Tolerance Calculation**:
```csharp
public double MinVoltage => ExpectedVoltage * (1 - TolerancePercent / 100) - AbsoluteTolerance;
public double MaxVoltage => ExpectedVoltage * (1 + TolerancePercent / 100) + AbsoluteTolerance;
```

### 4. Hardware Simulation

#### PowerSupply Class
**Simulates**: TTi PLH120-P DC Power Supply
- Voltage range: 0-120V
- Current limit: 0-0.75A
- Realistic switching delays (50-100ms)

#### DigitalMultiMeter Class
**Simulates**: Keysight 34465A Digital Multimeter
- Valid ranges: 0.1V, 1.0V, 10.0V, 100.0V, 1000.0V
- NPLC-based measurement timing
- Realistic measurement error (±2%)

## Test Sequence

### Test Matrix (8 combinations)

| Test | F1 | F2 | F3 | R_eff (kΩ)|Expected Output |
|------|----|----|----|----------------------------|
| 01   | 0V | 0V | 0V |     ∞     | ~3.30V         |
| 02   | 24V| 0V | 0V |    12.00  | ~2.37V         |
| 03   | 0V | 24V| 0V |    24.00  | ~2.76V         |
| 04   | 24V| 24V| 0V |    8.00   | ~2.08V         |
| 05   | 0V | 0V | 24V|    47.00  | ~3.00V         |
| 06   | 24V| 0V | 24V|    9.56   | ~2.21V         |
| 07   | 0V | 24V| 24V|    15.89  | ~2.55V         |
| 08   | 24V| 24V| 24V|    6.83   | ~1.95V         |


### Test Execution Flow
1. **Initialization**: Enable all power supplies
2. **For each test configuration**:
   - Set F1, F2, F3 voltages according to test state
   - Wait 100ms for stabilization
   - Measure output voltage with multimeter
   - Compare against expected range
   - Report result
3. **Cleanup**: Disable all outputs and reset voltages

## User Interface

### MainForm Components
- **Status Label**: Shows current test progress
- **Progress Bar**: Visual progress indicator
- **Start Button**: Initiates test sequence
- **Cancel Button**: Stops running tests
- **Output TextBox**: Displays test results in terminal style


## Error Handling

### Exception Types Handled
- `ArgumentOutOfRangeException`: Invalid voltage/current settings
- `ArgumentException`: Invalid measurement parameters
- `InvalidOperationException`: Hardware operation failures
- `OperationCanceledException`: Test cancellation

### Error Recovery Strategy
- Individual test failures don't stop the sequence
- Hardware errors are logged and reported
- All outputs are safely disabled in finally blocks

## Configuration & Extensibility

### Adding New Test Cases
1. Modify `CreateTestConfigurations()` method
2. Add new resistance values or voltage levels
3. Update UI progress bar maximum value

### Hardware Integration
- Implement `IPowerSupplyDc` for real power supplies
- Implement `IMultiMeter` for real measurement devices
- Replace simulation classes with actual hardware drivers

### Customization Points
- Test tolerances in `TestConfiguration`
- Measurement parameters (range, NPLC) in service
- Circuit parameters in `ResistorNetwork`

## Performance Characteristics

### Timing
- Single test duration: ~170ms (100ms stabilization + 20ms measurement + delays)
- Complete test suite: ~1.5 seconds (8 tests × ~200ms average)
- UI update frequency: Real-time with thread-safe invoke

### Memory Usage
- Minimal memory footprint
- No persistent data storage
- Efficient event-driven architecture

## Future Enhancements

### Potential Improvements
1. **Data Export**: Save results to CSV/Excel
2. **Test History**: Store and compare previous test runs
3. **Advanced Analytics**: Statistical analysis of measurements
4. **Configuration Files**: External test parameter configuration
5. **Logging Framework**: Structured logging with NLog/Serilog
6. **Unit Testing**: Comprehensive test coverage
7. **Modern UI**: Upgrade to WPF or WinUI 3

### Scalability Considerations
- Add support for multiple resistance networks
- Implement test scripting capabilities
- Add remote monitoring and control features
