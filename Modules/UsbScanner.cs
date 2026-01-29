using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class UsbScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "USB Scanner" };

            await Task.Run(() =>
            {
                var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable);
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        try
                        {
                            var files = Directory.GetFiles(drive.RootDirectory.FullName, "*.*", SearchOption.AllDirectories);
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
                                    result.Details.Add($"Suspicious file on USB ({drive.Name}): {fileName}");
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            });

            return result;
        }
    }
}
