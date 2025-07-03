using System;

namespace ResistanceNetworkTester.Models
{
    /// <summary>
    /// Contains results of a single test execution
    /// </summary>
    public class TestResult
    {
        /// <summary>Test configuration used</summary>
        public TestConfiguration Configuration { get; set; }
        
        /// <summary>Measured voltage in volts</summary>
        public double MeasuredVoltage { get; set; }
        
        /// <summary>Test pass/fail status</summary>
        public bool Passed { get; set; }
        
        /// <summary>Result message or error description</summary>
        public string Message { get; set; }
        
        /// <summary>Timestamp of test execution</summary>
        public DateTime Timestamp { get; set; }
    }
}