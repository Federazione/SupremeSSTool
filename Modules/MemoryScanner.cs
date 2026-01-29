using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;
using SupremeScreenSharePro.Utils;

namespace SupremeScreenSharePro.Modules
{
    public class MemoryScanner
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_READ = 0x0010;

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            public ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "Memory Scanner" };
            var targetProcesses = Process.GetProcessesByName("javaw");

            foreach (var process in targetProcesses)
            {
                var processInfo = new ProcessInfo
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    Path = GetProcessPath(process)
                };

                var foundStrings = await Task.Run(() => ScanProcessMemory(process));
                if (foundStrings.Count > 0)
                {
                    processInfo.FoundStrings.AddRange(foundStrings);
                    processInfo.RiskLevel = 1.0;
                    result.SuspiciousProcesses.Add(processInfo);
                    result.ThreatsFound++;
                    result.Details.Add($"Found {foundStrings.Count} suspicious strings in process {process.ProcessName} (PID: {process.Id})");
                }
            }

            return result;
        }

        private List<string> ScanProcessMemory(Process process)
        {
            var foundStrings = new List<string>();
            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, process.Id);

            if (processHandle == IntPtr.Zero)
                return foundStrings;

            try
            {
                SYSTEM_INFO sysInfo;
                GetSystemInfo(out sysInfo);

                IntPtr minAddress = sysInfo.minimumApplicationAddress;
                IntPtr maxAddress = sysInfo.maximumApplicationAddress;
                long currentAddress = (long)minAddress;

                while (currentAddress < (long)maxAddress)
                {
                    MEMORY_BASIC_INFORMATION memInfo;
                    VirtualQueryEx(processHandle, (IntPtr)currentAddress, out memInfo, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

                    if (memInfo.State == 0x1000 && (memInfo.Protect == 0x04 || memInfo.Protect == 0x02 || memInfo.Protect == 0x20))
                    {
                        byte[] buffer = new byte[(int)memInfo.RegionSize];
                        int bytesRead;
                        if (ReadProcessMemory(processHandle, (IntPtr)currentAddress, buffer, buffer.Length, out bytesRead))
                        {
                            string content = Encoding.ASCII.GetString(buffer);
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
                    }

                    currentAddress += (long)memInfo.RegionSize;
                }
            }
            finally
            {
                CloseHandle(processHandle);
            }

            return foundStrings;
        }

        private string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
