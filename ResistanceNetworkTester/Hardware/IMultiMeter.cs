using ResistanceNetworkTester.Models;

namespace ResistanceNetworkTester.Hardware
{
    /// <summary>
    /// Defines multimeter functionality
    /// </summary>
    public interface IMultiMeter
    {
        /// <summary>
        /// Reads voltage with specified range and integration time
        /// </summary>
        /// <param name="measurementRange">Measurement range</param>
        /// <param name="nplc">Integration time setting</param>
        /// <param name="config">Test configuration (optional)</param>
        /// <returns>Measured voltage</returns>
        double ReadVoltage(double measurementRange, NumberOfPowerLineCycles nplc, TestConfiguration config = null);
    }

    /// <summary>
    /// Number of power line cycles for integration time
    /// </summary>
    public enum NumberOfPowerLineCycles
    {
        /// <summary>40μs integration time</summary>
        Nplc0p002,
        /// <summary>1.2ms integration time</summary>
        Nplc0p06,
        /// <summary>20ms integration time</summary>
        Nplc1,
        /// <summary>200ms integration time</summary>
        Nplc10
    }
}