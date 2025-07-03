using System;

namespace ResistanceNetworkTester.Models
{
    /// <summary>
    /// Models the resistor network and calculates expected output voltage
    /// based on fuse states (F1, F2, F3)
    /// </summary>
    public class ResistorNetwork
    {
        /// <summary>Resistance for F1 path (12 kΩ)</summary>
        public double R_F1 { get; } = 12_000;
        
        /// <summary>Resistance for F2 path (24 kΩ)</summary>
        public double R_F2 { get; } = 24_000;
        
        /// <summary>Resistance for F3 path (47 kΩ)</summary>
        public double R_F3 { get; } = 47_000;
        
        /// <summary>Pull-up resistance (4.7 kΩ)</summary>
        public double R_Pullup { get; } = 4_700;
        
        /// <summary>Supply voltage (3.3V)</summary>
        public double Vcc { get; } = 3.3;

        /// <summary>
        /// Calculates expected voltage at ANALOGEINGANG
        /// </summary>
        /// <param name="f1">Fuse 1 state (true = intact)</param>
        /// <param name="f2">Fuse 2 state (true = intact)</param>
        /// <param name="f3">Fuse 3 state (true = intact)</param>
        /// <returns>Expected voltage in volts</returns>
        /// <remarks>
        /// Uses voltage divider formula with parallel combination of active resistors
        /// </remarks>
        public double CalculateOutput(bool f1, bool f2, bool f3)
        {
            // Calculate parallel resistance of active paths
            double conductance = 0;

            if (f1) conductance += 1.0 / R_F1;
            if (f2) conductance += 1.0 / R_F2;
            if (f3) conductance += 1.0 / R_F3;

            if (conductance == 0) 
                return Vcc;  // No pull-down resistors connected
            
            double R_parallel = 1.0 / conductance;

            // Voltage divider calculation
            return Vcc * R_parallel / (R_Pullup + R_parallel);
        }
    }
}