using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class BamScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "BAM Scanner" };

            await Task.Run(() =>
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    string bamPath = @"SYSTEM\CurrentControlSet\Services\bam\State\UserSettings";
                    var sids = RegistryHelper.GetSubKeyNames(hklm, bamPath);

                    foreach (var sid in sids)
                    {
                        var values = RegistryHelper.GetValues(hklm, $@"{bamPath}\{sid}");
                        foreach (var kvp in values)
                        {
                            string entryPath = kvp.Key.ToLower();
                            if (CheatDatabase.SuspiciousFileNames.Any(s => entryPath.Contains(s.ToLower())) ||
                                CheatDatabase.SuspiciousProcessNames.Any(s => entryPath.Contains(s.ToLower())))
                            {
                                result.RegistryTraces.Add(new RegistryRecord
                                {
                                    KeyPath = $@"{bamPath}\{sid}",
                                    ValueName = kvp.Key,
                                    ValueData = kvp.Value?.ToString(),
                                    LastWriteTime = DateTime.Now 
                                });
                                result.ThreatsFound++;
                                result.Details.Add($"Suspicious BAM entry found: {entryPath}");
                            }
                        }
                    }
                }
            });

            return result;
        }
    }
}
