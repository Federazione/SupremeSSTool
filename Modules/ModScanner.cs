using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class ModScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Mod Scanner" };
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string modsPath = Path.Combine(appData, ".minecraft", "mods");

            await Task.Run(() =>
            {
                if (Directory.Exists(modsPath))
                {
                    var files = Directory.GetFiles(modsPath);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        string fileName = fileInfo.Name.ToLower();

                        if (CheatDatabase.SuspiciousFileNames.Any(s => fileName.Contains(s.ToLower())) ||
                            CheatDatabase.SuspiciousStrings.Any(s => fileName.Contains(s.ToLower())))
                        {
                            result.SuspiciousFiles.Add(new FileRecord
                            {
                                FileName = fileInfo.Name,
                                FilePath = fileInfo.FullName,
                                Size = fileInfo.Length,
                                LastModified = fileInfo.LastWriteTime
                            });
                            result.ThreatsFound++;
                            result.Details.Add($"Suspicious mod file found: {fileInfo.Name}");
                        }
                    }
                }
            });

            return result;
        }
    }
}
