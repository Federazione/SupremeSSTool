using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class BrowserScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Browser Scanner" };
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(userProfile, "Downloads");

            await Task.Run(() =>
            {
                if (Directory.Exists(downloadsPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(downloadsPath, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            string fileName = Path.GetFileName(file).ToLower();
                            if (CheatDatabase.SuspiciousFileNames.Any(s => fileName.Contains(s.ToLower())))
                            {
                                result.SuspiciousFiles.Add(new FileRecord
                                {
                                    FileName = fileName,
                                    FilePath = file,
                                    Size = new FileInfo(file).Length,
                                    LastModified = File.GetLastWriteTime(file)
                                });
                                result.ThreatsFound++;
                                result.Details.Add($"Suspicious file in Downloads: {fileName}");
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
