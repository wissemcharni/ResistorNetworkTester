namespace ResistanceNetworkTester.Hardware
{
    /// <summary>
    /// Defines DC power supply functionality
    /// </summary>
    public interface IPowerSupplyDc
    {
        /// <summary>Enables power output</summary>
        void EnableOutput();
        
        /// <summary>Disables power output</summary>
        void DisableOutput();
        
        /// <summary>Sets output voltage</summary>
        /// <param name="voltage">Voltage in volts</param>
        void SetVoltage(double voltage);
        
        /// <summary>Sets current limit</summary>
        /// <param name="current">Current in amperes</param>
        void SetCurrent(double current);
        
        /// <summary>Sets power limit</summary>
        /// <param name="power">Power in watts</param>
        void SetPower(double power);
        
        /// <summary>Reads actual output voltage</summary>
        /// <returns>Measured voltage in volts</returns>
        double ReadVoltage();
    }
}