using Microsoft.Win32;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class UserAssistScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "UserAssist Scanner" };

            await Task.Run(() =>
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist"))
                {
                    if (key != null)
                    {
                        foreach (var subKeyName in key.GetSubKeyNames())
                        {
                            using (var subKey = key.OpenSubKey(subKeyName + @"\Count"))
                            {
                                if (subKey != null)
                                {
                                    foreach (var valueName in subKey.GetValueNames())
                                    {
                                        string decodedName = ROT13(valueName);
                                        if (CheatDatabase.SuspiciousFileNames.Any(s => decodedName.ToLower().Contains(s.ToLower())) ||
                                            CheatDatabase.SuspiciousProcessNames.Any(s => decodedName.ToLower().Contains(s.ToLower())))
                                        {
                                            result.RegistryTraces.Add(new RegistryRecord
                                            {
                                                KeyPath = subKey.Name,
                                                ValueName = decodedName,
                                                ValueData = "Execution Trace",
                                                LastWriteTime = DateTime.Now
                                            });
                                            result.ThreatsFound++;
                                            result.Details.Add($"Suspicious UserAssist entry: {decodedName}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            return result;
        }

        private string ROT13(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            char[] buffer = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= 'a' && c <= 'z')
                {
                    if (c > 'm') c -= (char)13;
                    else c += (char)13;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (c > 'M') c -= (char)13;
                    else c += (char)13;
                }
                buffer[i] = c;
            }
            return new string(buffer);
        }
    }
}
