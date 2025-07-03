using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ResistanceNetworkTester.Hardware; 
using ResistanceNetworkTester.Models;

namespace ResistanceNetworkTester.Services
{
    /// <summary>
    /// Orchestrates the test sequence for the resistor network
    /// </summary>
    public class ResistanceNetworkTesterService
    {
        private readonly IPowerSupplyDc powerSupplyF1;
        private readonly IPowerSupplyDc powerSupplyF2;
        private readonly IPowerSupplyDc powerSupplyF3;
        private readonly IMultiMeter multiMeter;
        private readonly List<TestConfiguration> testConfigurations;
        private CancellationTokenSource cancellationTokenSource;
        
        /// <summary>Event fired when a test result is available</summary>
        public event Action<TestResult> TestResultAvailable;
        
        /// <summary>Event fired for status updates</summary>
        public event Action<string> StatusChanged;

        /// <summary>
        /// Initializes hardware interfaces and test configurations
        /// </summary>
        public ResistanceNetworkTesterService()
        {
            powerSupplyF1 = new PowerSupply();
            powerSupplyF2 = new PowerSupply();
            powerSupplyF3 = new PowerSupply();
            multiMeter = new DigitalMultiMeter();
            testConfigurations = CreateTestConfigurations();
        }

        /// <summary>Read-only access to test configurations</summary>
        public IReadOnlyList<TestConfiguration> TestConfigurations => testConfigurations.AsReadOnly();

        /// <summary>
        /// Creates all 8 test configurations for fuse state combinations
        /// </summary>
        /// <returns>List of test configurations</returns>
        private List<TestConfiguration> CreateTestConfigurations()
        {
            var configs = new List<TestConfiguration>();
            var network = new ResistorNetwork();

            for (int i = 0; i < 8; i++)
            {
                bool f1 = (i & 1) != 0;
                bool f2 = (i & 2) != 0;
                bool f3 = (i & 4) != 0;

                configs.Add(new TestConfiguration
                {
                    TestName = $"Test_{i + 1:D2}",
                    F1_State = f1,
                    F2_State = f2,
                    F3_State = f3,
                    ExpectedVoltage = network.CalculateOutput(f1, f2, f3),
                    TolerancePercent = 2.0,  // ±2% tolerance
                    AbsoluteTolerance = 0.005 // ±5mV absolute tolerance
                });
            }

            return configs;
        }

        /// <summary>
        /// Executes the complete test sequence asynchronously
        /// </summary>
        /// <remarks>
        /// Runs all test configurations in order with cancellation support
        /// </remarks>
        public async Task StartTestAsync()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            try
            {
                StatusChanged?.Invoke("Initializing instruments...");
                StatusChanged?.Invoke("Starting test sequence...");

                for (int i = 0; i < testConfigurations.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    StatusChanged?.Invoke($"Test {i + 1}/{testConfigurations.Count}: {testConfigurations[i]}");

                    var result = await RunSingleTestAsync(testConfigurations[i], token);
                    TestResultAvailable?.Invoke(result);

                    await Task.Delay(300, token); // Short delay between tests
                }

                StatusChanged?.Invoke(!token.IsCancellationRequested 
                    ? "All tests completed!" 
                    : "Tests cancelled");
            }
            catch (OperationCanceledException)
            {
                StatusChanged?.Invoke("Tests cancelled");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Error: {ex.Message}");
            }
            finally
            {
                SafePowerDown();
            }
        }

        /// <summary>
        /// Executes a single test configuration
        /// </summary>
        /// <param name="config">Test configuration to execute</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Test result with measurements</returns>
        private async Task<TestResult> RunSingleTestAsync(TestConfiguration config, CancellationToken token)
        {
            var result = new TestResult
            {
                Configuration = config,
                Timestamp = DateTime.Now
            };

            try
            {
                // Configure power supplies
                await Task.Run(() =>
                {
                    powerSupplyF1.SetVoltage(config.F1_State ? 24 : 0);
                    powerSupplyF2.SetVoltage(config.F2_State ? 24 : 0);
                    powerSupplyF3.SetVoltage(config.F3_State ? 24 : 0);
                    
                    powerSupplyF1.EnableOutput();
                    powerSupplyF2.EnableOutput();
                    powerSupplyF3.EnableOutput();
                }, token);

                await Task.Delay(100, token); // Stabilization delay

                // Auto-range voltmeter based on expected voltage
                double range = config.ExpectedVoltage switch
                {
                    < 1.0 => 1.0,    // 1V range
                    < 10.0 => 10.0,  // 10V range
                    _ => 100.0       // 100V range
                };

                result.MeasuredVoltage = await Task.Run(() => 
                    multiMeter.ReadVoltage(range, NumberOfPowerLineCycles.Nplc1, config), token);

                // Evaluate result against tolerance window
                result.Passed = result.MeasuredVoltage >= config.MinVoltage &&
                               result.MeasuredVoltage <= config.MaxVoltage;

                result.Message = result.Passed
                    ? "PASS"
                    : $"FAIL (Expected: {config.ExpectedVoltage:F3}V)";
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Error: {ex.Message}";
            }
            finally
            {
                SafePowerDown();
            }

            return result;
        }

        /// <summary>
        /// Safely disables all power supply outputs
        /// </summary>
        private void SafePowerDown()
        {
            try
            {
                powerSupplyF1.DisableOutput();
                powerSupplyF2.DisableOutput();
                powerSupplyF3.DisableOutput();
                powerSupplyF1.SetVoltage(0);
                powerSupplyF2.SetVoltage(0);
                powerSupplyF3.SetVoltage(0);
            }
            catch { /* Suppress shutdown errors */ }
        }

        /// <summary>
        /// Cancels the running test sequence
        /// </summary>
        public void CancelTest()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}