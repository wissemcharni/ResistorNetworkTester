namespace ResistanceNetworkTester.Models
{
    /// <summary>
    /// Represents a test configuration with expected values and tolerances
    /// </summary>
    public class TestConfiguration
    {
        /// <summary>Test name/identifier</summary>
        public string TestName { get; set; }
        
        /// <summary>Fuse 1 state (true = 24V applied)</summary>
        public bool F1_State { get; set; }
        
        /// <summary>Fuse 2 state (true = 24V applied)</summary>
        public bool F2_State { get; set; }
        
        /// <summary>Fuse 3 state (true = 24V applied)</summary>
        public bool F3_State { get; set; }
        
        /// <summary>Expected voltage in volts</summary>
        public double ExpectedVoltage { get; set; }
        
        /// <summary>Allowed tolerance percentage</summary>
        public double TolerancePercent { get; set; } = 2.0;
        
        /// <summary>Absolute tolerance in volts</summary>
        public double AbsoluteTolerance { get; set; } = 0.005;
        
        /// <summary>Minimum acceptable voltage</summary>
        public double MinVoltage => 
            ExpectedVoltage * (1 - TolerancePercent / 100) - AbsoluteTolerance;
        
        /// <summary>Maximum acceptable voltage</summary>
        public double MaxVoltage => 
            ExpectedVoltage * (1 + TolerancePercent / 100) + AbsoluteTolerance;

        /// <summary>
        /// Returns string representation of fuse states
        /// </summary>
        public override string ToString() => 
            $"F1:{(F1_State ? "24V" : "0V")} " +
            $"F2:{(F2_State ? "24V" : "0V")} " +
            $"F3:{(F3_State ? "24V" : "0V")}";
    }
}