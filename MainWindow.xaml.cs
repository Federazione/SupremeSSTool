using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Modules;

namespace SupremeScreenSharePro
{
    public partial class MainWindow : Window
    {
        private List<ScanResult> _scanResults;
        private int _totalThreats;

        public MainWindow()
        {
            InitializeComponent();
            _scanResults = new List<ScanResult>();
            _totalThreats = 0;
            Log("Ready for scanning.");
        }

        private void Log(string message)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.ScrollToEnd();
        }

        private void UpdateStatus(string status)
        {
            lblStatus.Content = status;
        }

        private bool CheckPrerequisites()
        {
            var processes = Process.GetProcessesByName("javaw");
            if (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("java");
            }

            if (processes.Length == 0)
            {
                var result = MessageBox.Show(
                    "Minecraft (javaw.exe/java.exe) is not running. Some scans may be ineffective.\nDo you want to continue anyway?",
                    "Prerequisite Warning",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    Log("Scan aborted by user due to missing prerequisites.");
                    return false;
                }
                Log("Warning: Minecraft not running. Continuing scan as requested.");
            }
            return true;
        }

        private async Task RunScan(Func<Task<ScanResult>> scanAction, string scanName, bool checkPrerequisites = true)
        {
            if (checkPrerequisites && !CheckPrerequisites()) return;

            try
            {
                UpdateStatus($"Scanning: {scanName}...");
                progressBar.IsIndeterminate = true;
                Log($"Starting {scanName}...");

                var result = await scanAction();

                _scanResults.Add(result);
                _totalThreats += result.ThreatsFound;
                lblRedFlags.Content = $"Red Flags: {_totalThreats}";

                if (result.ThreatsFound > 0)
                {
                    Log($"[ALERT] {scanName} found {result.ThreatsFound} threats!");
                    foreach (var detail in result.Details)
                    {
                        Log($"  ! {detail}");
                    }
                }
                else
                {
                    Log($"{scanName} clean.");
                }
            }
            catch (Exception ex)
            {
                Log($"Error in {scanName}: {ex.Message}");
            }
            finally
            {
                progressBar.IsIndeterminate = false;
                UpdateStatus("Ready");
            }
        }

        private async void BtnInjectionScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new InjectionDetector().ScanAsync, "Injection Scan");
        }

        private async void BtnBamScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new BamScanner().ScanAsync, "BAM Scan");
        }

        private async void BtnModScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new ModScanner().ScanAsync, "Mod Scan");
        }

        private async void BtnUsbScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new UsbScanner().ScanAsync, "USB Scan");
        }

        private async void BtnProcessScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new ProcessAnalyzer().ScanAsync, "Process Scan");
        }

        private async void BtnRecycleScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new RecycleBinScanner().ScanAsync, "Recycle Bin Scan");
        }

        private async void BtnUserAssistScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new UserAssistScanner().ScanAsync, "UserAssist Scan");
        }

        private async void BtnBrowserScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new BrowserScanner().ScanAsync, "Browser Scan");
        }

        private async void BtnMemoryScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new MemoryScanner().ScanAsync, "Memory Scan");
        }

        private async void BtnUsnScan_Click(object sender, RoutedEventArgs e)
        {
            await RunScan(new UsnJournalScanner().ScanAsync, "USN Journal Scan");
        }

        private async void BtnUltraScan_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckPrerequisites()) return;

            _scanResults.Clear();
            _totalThreats = 0;
            txtLog.Clear();
            Log("Starting ULTRA SCAN (All Modules)...");

            await RunScan(new InjectionDetector().ScanAsync, "Injection Scan", false);
            await RunScan(new BamScanner().ScanAsync, "BAM Scan", false);
            await RunScan(new ModScanner().ScanAsync, "Mod Scan", false);
            await RunScan(new ProcessAnalyzer().ScanAsync, "Process Scan", false);
            await RunScan(new MemoryScanner().ScanAsync, "Memory Scan", false);
            await RunScan(new UsbScanner().ScanAsync, "USB Scan", false);
            await RunScan(new RecycleBinScanner().ScanAsync, "Recycle Bin Scan", false);
            await RunScan(new UserAssistScanner().ScanAsync, "UserAssist Scan", false);
            await RunScan(new BrowserScanner().ScanAsync, "Browser Scan", false);
            await RunScan(new UsnJournalScanner().ScanAsync, "USN Journal Scan", false);

            Log("Ultra Scan Complete.");
            GenerateReport();
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            try
            {
                var generator = new ReportGenerator();
                string html = generator.GenerateHtmlReport(_scanResults);
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"ScanReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                File.WriteAllText(path, html);
                Log($"Report generated: {path}");
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log($"Error generating report: {ex.Message}");
            }
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Clear();
            _scanResults.Clear();
            _totalThreats = 0;
            lblRedFlags.Content = "Red Flags: 0";
            Log("Log cleared.");
        }

        private void BtnSelfDestruct_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to self-destruct? This will delete the application.", "Confirm Self-Destruct", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private async void BtnUserAssist_Click(object sender, RoutedEventArgs e)
        {
             await RunScan(new UserAssistScanner().ScanAsync, "UserAssist Scan");
        }
    }
}
