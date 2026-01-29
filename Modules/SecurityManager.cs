using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;

namespace SupremeScreenSharePro.Modules
{
    public static class SecurityManager
    {
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void SelfDestruct()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule.FileName;
                string batchPath = Path.Combine(Path.GetTempPath(), "cleanup.bat");
                string batchCommands = "@echo off\r\n" +
                                     "timeout /t 3 /nobreak > nul\r\n" +
                                     $"del /f /q \"{exePath}\"\r\n" +
                                     $"del /f /q \"{batchPath}\" & exit\r\n";

                File.WriteAllText(batchPath, batchCommands);

                Process.Start(new ProcessStartInfo
                {
                    FileName = batchPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                Application.Current.Shutdown();
            }
            catch
            {
            }
        }
    }
}
