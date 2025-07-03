using System;
using System.Drawing;
using System.Windows.Forms;
using ResistanceNetworkTester.Models; 
using ResistanceNetworkTester.Services;

namespace ResistanceNetworkTester.UI
{
    /// <summary>
    /// Main application form for Resistance Network Tester
    /// </summary>
    public partial class MainForm : Form
    {
        private ResistanceNetworkTesterService tester;
        private Button startButton;
        private Button cancelButton;
        private RichTextBox outputTextBox;
        private Label statusLabel;
        private ProgressBar progressBar;
        private bool testRunning = false;

        /// <summary>
        /// Initializes UI components and test service
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            tester = new ResistanceNetworkTesterService();
            tester.TestResultAvailable += OnTestResultAvailable;
            tester.StatusChanged += OnStatusChanged;
        }

        /// <summary>
        /// Initializes UI controls and layout
        /// </summary>
        private void InitializeComponent()
        {
            // Form setup
            this.Text = "Resistance Network Tester";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);

            // Status label
            statusLabel = new Label
            {
                Text = "Ready to test",
                Location = new Point(15, 15),
                Size = new Size(850, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.LightGreen
            };

            // Progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(15, 45),
                Size = new Size(850, 25),
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.LimeGreen
            };

            // Start button
            startButton = new Button
            {
                Text = "Start Test",
                Location = new Point(15, 80),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White
            };
            startButton.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
            startButton.Click += StartButton_Click;

            // Cancel button
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(150, 80),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(200, 73, 73),
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(200, 73, 73);
            cancelButton.Click += CancelButton_Click;

            // Output console
            outputTextBox = new RichTextBox
            {
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Location = new Point(15, 130),
                Size = new Size(850, 480),
                Font = new Font("Consolas", 9F),
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                DetectUrls = false
            };

            // Add controls to form
            this.Controls.Add(statusLabel);
            this.Controls.Add(progressBar);
            this.Controls.Add(startButton);
            this.Controls.Add(cancelButton);
            this.Controls.Add(outputTextBox);

            // Initial message
            AppendOutput("=== RESISTANCE NETWORK TESTER ===", Color.Cyan);
            AppendOutput("Ready to start test sequence", Color.LightGreen);
            AppendOutput($"Created {DateTime.Today:yyyy-MM-dd}", Color.Gray);
        }

        /// <summary>
        /// Handles Start Test button click
        /// </summary>
        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (testRunning) return;

            testRunning = true;
            startButton.Enabled = false;
            cancelButton.Enabled = true;
            progressBar.Value = 0;
            progressBar.Maximum = tester.TestConfigurations.Count;

            outputTextBox.Clear();
            AppendOutput($"=== TEST STARTED: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===", Color.Cyan);
            
            await tester.StartTestAsync();
            
            testRunning = false;
            startButton.Enabled = true;
            cancelButton.Enabled = false;
        }

        /// <summary>
        /// Handles Cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e) => tester.CancelTest();

        /// <summary>
        /// Processes incoming test results for UI display
        /// </summary>
        /// <param name="result">Test result to display</param>
        private void OnTestResultAvailable(TestResult result)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<TestResult>(OnTestResultAvailable), result);
                return;
            }

            progressBar.Value++;
            
            Color statusColor = result.Passed ? Color.LimeGreen : Color.Tomato;
            string status = result.Passed ? "PASS" : "FAIL";
            
            AppendOutput($"{result.Timestamp:HH:mm:ss.fff} | {result.Configuration.TestName}", 
                         result.Passed ? Color.White : Color.Yellow);
            
            AppendOutput($"  F1: {result.Configuration.F1_State} | " +
                         $"F2: {result.Configuration.F2_State} | " +
                         $"F3: {result.Configuration.F3_State}", 
                         Color.LightGray);
            
            AppendOutput($"  Measured: {result.MeasuredVoltage:F3}V | " +
                         $"Expected: {result.Configuration.ExpectedVoltage:F3}V | " +
                         $"Status: {status}", 
                         statusColor);
            
            if (!result.Passed)
            {
                AppendOutput($"  Tolerance: ±{result.Configuration.TolerancePercent}% " +
                             $"[{result.Configuration.MinVoltage:F3}V - " +
                             $"{result.Configuration.MaxVoltage:F3}V]", 
                             Color.Yellow);
                AppendOutput($"  Error: {result.Message}", Color.Tomato);
            }
        }

        /// <summary>
        /// Updates status messages
        /// </summary>
        /// <param name="status">New status message</param>
        private void OnStatusChanged(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(OnStatusChanged), status);
                return;
            }

            statusLabel.Text = status;
            AppendOutput($"[STATUS] {status}", Color.LightBlue);
        }

        /// <summary>
        /// Appends colored text to the output console
        /// </summary>
        /// <param name="text">Text to append</param>
        /// <param name="color">Text color</param>
        private void AppendOutput(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color>(AppendOutput), text, color);
                return;
            }

            outputTextBox.SelectionStart = outputTextBox.TextLength;
            outputTextBox.SelectionColor = color;
            outputTextBox.AppendText(text + Environment.NewLine);
            outputTextBox.SelectionColor = outputTextBox.ForeColor; // Reset color
            outputTextBox.ScrollToCaret();
        }
    }
}