using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class InjectionDetector
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Injection Detector" };
            var targetProcesses = Process.GetProcessesByName("javaw");

            await Task.Run(() =>
            {
                foreach (var process in targetProcesses)
                {
                    try
                    {
                        foreach (ProcessModule module in process.Modules)
                        {
                            string moduleName = module.ModuleName.ToLower();
                            if (CheatDatabase.SuspiciousFileNames.Any(s => moduleName.Contains(s.ToLower())) ||
                                CheatDatabase.SuspiciousStrings.Any(s => moduleName.Contains(s.ToLower())))
                            {
                                result.SuspiciousProcesses.Add(new ProcessInfo
                                {
                                    Id = process.Id,
                                    Name = process.ProcessName,
                                    Path = module.FileName,
                                    RiskLevel = 1.0
                                });
                                result.ThreatsFound++;
                                result.Details.Add($"Suspicious DLL injected in javaw.exe: {module.ModuleName}");
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            });

            return result;
        }
    }
}
