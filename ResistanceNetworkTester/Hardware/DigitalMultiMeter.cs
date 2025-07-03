using System;
using System.Linq;
using System.Threading;
using ResistanceNetworkTester.Models;

namespace ResistanceNetworkTester.Hardware
{
    /// <summary>
    /// Simulates a digital multimeter with realistic measurement characteristics
    /// </summary>
    public class DigitalMultiMeter : IMultiMeter
    {
        /// <summary>Valid measurement ranges in volts</summary>
        private static readonly double[] ValidRanges = { 0.1, 1.0, 10.0, 100.0, 1000.0 };
        
        private readonly Random random = new Random();

        /// <summary>
        /// Simulates voltage measurement with configurable range and integration time
        /// </summary>
        /// <param name="measurementRange">Selected measurement range</param>
        /// <param name="nplc">Number of power line cycles for integration</param>
        /// <param name="config">Test configuration (optional)</param>
        /// <returns>Measured voltage with simulated error</returns>
        /// <exception cref="ArgumentException">Thrown for invalid measurement range</exception>
        /// <remarks>
        /// Simulates measurement time based on NPLC setting and adds ±1% range-based error
        /// </remarks>
        public double ReadVoltage(double measurementRange, NumberOfPowerLineCycles nplc, TestConfiguration config = null)
        {
            // Validate measurement range
            if (!ValidRanges.Any(range => Math.Abs(measurementRange - range) < 0.001))
                throw new ArgumentException($"Invalid measurement range. Valid ranges: {string.Join(", ", ValidRanges)}V");

            // Simulate measurement time based on NPLC
            int delayMs = nplc switch
            {
                NumberOfPowerLineCycles.Nplc0p002 => 1,   // 40μs simulation
                NumberOfPowerLineCycles.Nplc0p06 => 2,    // 1.2ms simulation
                NumberOfPowerLineCycles.Nplc1 => 20,       // 20ms simulation
                NumberOfPowerLineCycles.Nplc10 => 200,      // 200ms simulation
                _ => 20
            };
            Thread.Sleep(delayMs);

            // Realistic error simulation (±1% of range)
            double error = (random.NextDouble() - 0.5) * 0.01 * measurementRange;
            double voltage = (config?.ExpectedVoltage ?? 0) + error;
            
            // Clamp to valid range
            return Math.Clamp(voltage, 0, measurementRange);
        }
    }
}