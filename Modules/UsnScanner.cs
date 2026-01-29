using System;
using System.Threading.Tasks;
using SupremeScreenSharePro.Models;

namespace SupremeScreenSharePro.Modules
{
    public class UsnJournalScanner
    {
        public async Task<ScanResult> ScanAsync()
        {
            var result = new ScanResult { ModuleName = "USN Journal Scanner" };
            await Task.Delay(100); 
            return result;
        }
    }
}
