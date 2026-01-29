using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class RecycleBinScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Recycle Bin Scanner" };
            
            await Task.Run(() =>
            {
                try
                {
                    string[] drives = Directory.GetLogicalDrives();
                    foreach (var drive in drives)
                    {
                        string recyclePath = Path.Combine(drive, "$Recycle.Bin");
                        if (Directory.Exists(recyclePath))
                        {
                            ScanDirectory(recyclePath, result);
                        }
                    }
                }
                catch
                {
                }
            });

            return result;
        }

        private void ScanDirectory(string path, ScanResult result)
        {
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    try
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
                            result.Details.Add($"Suspicious file in Recycle Bin: {fileName}");
                        }
                    }
                    catch { }
                }

                foreach (var dir in Directory.GetDirectories(path))
                {
                    ScanDirectory(dir, result);
                }
            }
            catch { }
        }
    }
}
