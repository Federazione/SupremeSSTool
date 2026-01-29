using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class ProcessAnalyzer
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Process Analyzer" };
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                try
                {
                    string processName = process.ProcessName.ToLower();
                    string windowTitle = process.MainWindowTitle.ToLower();
                    string path = GetProcessPath(process);

                    bool isSuspicious = false;
                    var processInfo = new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Path = path,
                        MainWindowTitle = process.MainWindowTitle
                    };

                    if (CheatDatabase.SuspiciousProcessNames.Any(s => processName.Contains(s.ToLower())))
                    {
                        isSuspicious = true;
                        result.Details.Add($"Suspicious process name: {process.ProcessName} (PID: {process.Id})");
                    }

                    if (!string.IsNullOrEmpty(windowTitle) && CheatDatabase.SuspiciousStrings.Any(s => windowTitle.Contains(s.ToLower())))
                    {
                        isSuspicious = true;
                        result.Details.Add($"Suspicious window title: {process.MainWindowTitle} (PID: {process.Id})");
                    }

                    if (!string.IsNullOrEmpty(path) && !path.StartsWith(@"C:\Windows", StringComparison.OrdinalIgnoreCase))
                    {
                        var fileStrings = await Task.Run(() => ScanFileForStrings(path));
                        if (fileStrings.Count > 0)
                        {
                            isSuspicious = true;
                            processInfo.FoundStrings.AddRange(fileStrings);
                            result.Details.Add($"Found {fileStrings.Count} suspicious strings in executable: {path}");
                        }
                    }

                    if (isSuspicious)
                    {
                        processInfo.RiskLevel = 1.0;
                        result.SuspiciousProcesses.Add(processInfo);
                        result.ThreatsFound++;
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        private string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch
            {
                return null;
            }
        }

        private List<string> ScanFileForStrings(string filePath)
        {
            var foundStrings = new List<string>();
            try
            {
                if (!File.Exists(filePath)) return foundStrings;

                byte[] bytes = File.ReadAllBytes(filePath);
                string content = Encoding.ASCII.GetString(bytes);

                foreach (var target in CheatDatabase.SuspiciousStrings)
                {
                    if (content.Contains(target))
                    {
                        if (!foundStrings.Contains(target))
                        {
                            foundStrings.Add(target);
                        }
                    }
                }
            }
            catch
            {
            }
            return foundStrings;
        }
    }
}
