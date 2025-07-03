using System;
using System.Threading;

namespace ResistanceNetworkTester.Hardware
{
    /// <summary>
    /// Simulates a DC power supply with configurable output
    /// </summary>
    public class PowerSupply : IPowerSupplyDc
    {
        private double currentVoltage = 0;

        /// <summary>
        /// Enables output with simulated delay
        /// </summary>
        public void EnableOutput()
        {
            Thread.Sleep(100); // Simulate enable delay
        }

        /// <summary>
        /// Disables output with simulated delay
        /// </summary>
        public void DisableOutput()
        {
            Thread.Sleep(100); // Simulate disable delay
        }

        /// <summary>
        /// Sets output voltage (0-120V range)
        /// </summary>
        /// <param name="voltage">Target voltage</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown for voltages outside 0-120V range
        /// </exception>
        public void SetVoltage(double voltage)
        {
            if (voltage < 0 || voltage > 120)
                throw new ArgumentOutOfRangeException("Voltage must be between 0 and 120V");
            
            currentVoltage = voltage;
            Thread.Sleep(50); // Simulate settling time
        }

        /// <summary>
        /// Sets current limit (0-0.75A range)
        /// </summary>
        /// <param name="current">Target current</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown for currents outside 0-0.75A range
        /// </exception>
        public void SetCurrent(double current)
        {
            if (current < 0 || current > 0.75)
                throw new ArgumentOutOfRangeException("Current must be between 0 and 0.75A");
        }

        /// <summary>
        /// Sets power limit (not implemented in simulation)
        /// </summary>
        /// <param name="power">Target power</param>
        public void SetPower(double power)
        {
            // Not implemented in simulation
        }

        /// <summary>
        /// Reads actual output voltage
        /// </summary>
        /// <returns>Current voltage setting</returns>
        public double ReadVoltage()
        {
            return currentVoltage;
        }
    }
}