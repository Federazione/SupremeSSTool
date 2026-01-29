using System;
using System.Collections.Generic;

namespace SupremeScreenSharePro.Models
{
    public class ScanResult
    {
        public string ModuleName { get; set; }
        public DateTime ScanTime { get; set; }
        public int ThreatsFound { get; set; }
        public List<string> Details { get; set; }
        public List<ProcessInfo> SuspiciousProcesses { get; set; }
        public List<FileRecord> SuspiciousFiles { get; set; }
        public List<RegistryRecord> RegistryTraces { get; set; }

        public ScanResult()
        {
            Details = new List<string>();
            SuspiciousProcesses = new List<ProcessInfo>();
            SuspiciousFiles = new List<FileRecord>();
            RegistryTraces = new List<RegistryRecord>();
            ScanTime = DateTime.Now;
        }
    }

    public class ProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string MainWindowTitle { get; set; }
        public double RiskLevel { get; set; }
        public List<string> FoundStrings { get; set; }

        public ProcessInfo()
        {
            FoundStrings = new List<string>();
        }
    }

    public class FileRecord
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }
    }

    public class RegistryRecord
    {
        public string KeyPath { get; set; }
        public string ValueName { get; set; }
        public string ValueData { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
